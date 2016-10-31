#region License
//Copyright(c) 2016 Dmytro Mukalov

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.
#endregion

namespace ObjectPort
{
    using System.Diagnostics;
    using Common;
    using Descriptions;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;

    // TODO
    // add supoort for Dictioanary
    // serializer configurable encoding
    // serializer parameter for type id
    // alphabetical order for members
    //+ optimize complex enumerable for struct (check for null)
    public sealed class Serializer
    {
        private static SerializerState _state = new SerializerState();
        private static readonly object Locker = new object();
        public static Encoding Encoding = Encoding.UTF8;


        public static void RegisterTypes(IEnumerable<Type> types)
        {
            lock (Locker)
            {
                var state = _state.Clone();

                var descriptions = new List<TypeDescription>();
                foreach (var type in types)
                {
                    var description = GetTypeDescription(type, state);
                    if (description != null)
                        descriptions.Add(description);
                }
                foreach (var description in descriptions)
                    description.Build();

                var maxId = state.AllTypeDescriptions.Max(i => i.Value.TypeId);
                state.DescriptionsById = new TypeDescription[maxId + 1];
                foreach (var item in state.AllTypeDescriptions)
                    state.DescriptionsById[item.Value.TypeId] = item.Value;
                Interlocked.Exchange(ref _state, state);
            }
        }

        public static void Serialize(Stream stream, object obj)
        {
            Debug.Assert(_state != null, "_state != null");
            var description = _state.GetDescription(obj.GetType());
            using (var writer = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                writer.Write(description.TypeId);
                description.Serialize(writer, obj);
            }
        }

        public static object Deserialize(Stream stream)
        {
            using (var reader = new BinaryReader(stream, Encoding.UTF8, true))
            {
                var state = _state;
                var typeId = reader.ReadInt16();
                if (typeId >= state.DescriptionsById.Length)
                    state.UnknownTypeId(typeId);
                var description = state.DescriptionsById[typeId];
                return description.Deserialize(reader);
            }
        }

        public static void Clear()
        {
            lock (Locker)
            {
                var state = new SerializerState();
                Interlocked.Exchange(ref _state, state);
            }
        }

        internal static TypeDescription GetTypeDescription(Type type, SerializerState state)
        {
            if (type.IsBuiltInType())
            {
                var nullableUnderlyingType = Nullable.GetUnderlyingType(type);
                if (nullableUnderlyingType != null)
                {
                    if (!nullableUnderlyingType.IsBuiltInType())
                    {
                        var td = GetTypeDescription(nullableUnderlyingType, state);
                        td?.Build();
                    }
                }
                return null;
            }

            TypeDescription description;
            if (state.AllTypeDescriptions.TryGetValue(type, out description))
                return description;

            if (type.IsDictionaryType())
            {
                var dictTypes = type.GetDictionaryArguments();
                var keyTd = GetTypeDescription(dictTypes.Item1, state);
                keyTd?.Build();
                var valTd = GetTypeDescription(dictTypes.Item2, state);
                valTd?.Build();
                return null;
            }

            if (type.IsEnumerableType())
            {
                var elementType = type.GetEnumerableArgument();
                if (!elementType.IsInterface && !elementType.IsAbstract)
                {
                    var td = GetTypeDescription(elementType, state);
                    td?.Build();
                }
                return null;
            }

            if (type.IsAnonymousType())
                description = new AnonymousTypeDescription(state.LastTypeId++, type, state);
            else
                description = new ComplexTypeDescription(state.LastTypeId++, type, state);
            state.AllTypeDescriptions.Add(type, description);
            state.AddDescription(type, description);
            return description;
        }
    }
}
