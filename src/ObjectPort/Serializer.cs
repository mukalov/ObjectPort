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
    using System.Reflection;
    using Builders;
    using System.Runtime.CompilerServices;
    using Attributes;
    using ObjectPort.Formatters;

    // TODO
    // optimize Array of primitives
    // serializer configurable encoding
    public sealed class Serializer
    {
        private static SerializerState _state = new SerializerState();
        private static readonly object Locker = new object();
        public static Encoding Encoding = Encoding.UTF8;

        public static void RegisterTypes(IEnumerable<Assembly> assemblies)
        {
            var allTypes = assemblies.SelectMany(a => a.GetTypes());
            var portableMarkedTypes = allTypes.Where(t => t.HasAttribute(typeof(PortableAttribute)));
            var portableInterfaces = portableMarkedTypes.Where(t => t.GetTypeInfo().IsInterface);
            var portableAbstractClasses = portableMarkedTypes.Where(t => t.GetTypeInfo().IsAbstract);
            var portableInterfaceImplementations = portableInterfaces
                .SelectMany(i => allTypes.Where(t => i.GetTypeInfo().IsAssignableFrom(t) && t.GetTypeInfo().IsClass && !t.GetTypeInfo().IsAbstract));
            var portableAbstractImplementations = portableAbstractClasses
                .SelectMany(a => allTypes.Where(t => t.GetTypeInfo().IsSubclassOf(a) && !t.GetTypeInfo().IsAbstract));
            var portableTypes = portableMarkedTypes
                .Union(portableInterfaceImplementations)
                .Union(portableAbstractImplementations);

            RegisterTypes(portableTypes);
        }

        public static void RegisterTypes(IEnumerable<Type> types)
        {
            lock (Locker)
            {
                var state = _state.Clone();

                RegisterPrimitiveTypes(state);
                var descriptions = new List<TypeDescription>();
                foreach (var type in types)
                {
                    if (type.IsNotComplexType())
                    {
                        var description = CreateTypeDescription(typeof(SimpleTypeDescription<>), type, state.LastTypeId++, type, state);
                        state.AddDescription(type, description);
                        descriptions.Add(description);
                    }
                    else
                    {
                        var description = GetTypeDescription(type, state);
                        if (description != null)
                            descriptions.Add(description);
                    }
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

        public static void RegisterTypes(IDictionary<ushort, Type> types)
        {
            lock (Locker)
            {
                var state = _state.Clone();

                RegisterPrimitiveTypes(state);
                var descriptions = new List<TypeDescription>();
                foreach (var type in types)
                {
                    if (type.Value.IsNotComplexType())
                    {
                        var description = CreateTypeDescription(typeof(SimpleTypeDescription<>), type.Value, type.Key, type.Value, state);
                        state.AddDescription(type.Value, description);
                        descriptions.Add(description);
                    }
                    else
                    {
                        if (type.Key >= SerializerState.CustomTypeIdsStart)
                            _state.InvalidTypeId();

                        var description = GetTypeDescription(type.Value, type.Key, state, false);
                        if (description != null)
                            descriptions.Add(description);
                    }
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
            Debug.Assert(_state != null, "State can't be null");

            if (obj == null)
                _state.InstanceCannotBeNull();

            var description = _state.GetDescription(obj.GetType());
            if (description == null)
                _state.TypeNotSupported(obj.GetType());

            var writer = WriterPool.GetFormatter(stream, Encoding);
            try
            {
                writer.Write(description.TypeId);
                description.Serialize(writer, obj);
            }
            finally
            {
                WriterPool.ReleaseFormatter(writer);
            }
        }

        public static void Serialize(BinaryWriter writer, object obj)
        {
            Debug.Assert(_state != null, "State can't be null");

            if (obj == null)
                _state.InstanceCannotBeNull();

            var description = _state.GetDescription(obj.GetType());
            if (description == null)
                _state.TypeNotSupported(obj.GetType());

            writer.Write(description.TypeId);
            description.Serialize(writer, obj);
        }

        public static void Serialize<T>(Stream stream, T obj)
        {
            Debug.Assert(_state != null, "State can't be null");

            if (obj == null)
                _state.InstanceCannotBeNull();

            var description = _state.GetDescription(RuntimeHelpers.GetHashCode(typeof(T)));
            if (description == null)
                _state.TypeNotSupported(obj.GetType());

            var writer = WriterPool.GetFormatter(stream, Encoding);
            try
            {
                writer.Write(description.TypeId);
                ((SpecializedTypeDescription<T>)description).SerializeHanlder(obj, writer);
            }
            finally
            {
                WriterPool.ReleaseFormatter(writer);
            }
        }

        public static void Serialize<T>(BinaryWriter writer, T obj)
        {
            Debug.Assert(_state != null, "State can't be null");

            if (obj == null)
                _state.InstanceCannotBeNull();

            var description = _state.GetDescription(RuntimeHelpers.GetHashCode(typeof(T)));
            if (description == null)
                _state.TypeNotSupported(obj.GetType());

            writer.Write(description.TypeId);
            ((SpecializedTypeDescription<T>)description).SerializeHanlder(obj, writer);
        }

        public static object Deserialize(Stream stream)
        {
            var reader = ReaderPool.GetFormatter(stream, Encoding);
            var state = _state;
            try
            {
                var typeId = reader.ReadInt16();
                if (typeId >= state.DescriptionsById.Length)
                    state.UnknownTypeId(typeId);

                var description = state.DescriptionsById[typeId];
                var obj = description.Deserialize(reader);
                return obj;
            }
            finally
            {
                ReaderPool.ReleaseFormatter(reader);
            }
        }

        public static object Deserialize(BinaryReader reader)
        {
            var state = _state;
            var typeId = reader.ReadInt16();
            if (typeId >= state.DescriptionsById.Length)
                state.UnknownTypeId(typeId);

            var description = state.DescriptionsById[typeId];
            var obj = description.Deserialize(reader);
            return obj;
        }

        public static T Deserialize<T>(Stream stream)
        {
            var reader = ReaderPool.GetFormatter(stream, Encoding);
            var state = _state;
            try
            {
                var typeId = reader.ReadInt16();
                if (typeId >= state.DescriptionsById.Length)
                    state.UnknownTypeId(typeId);

                var description = state.DescriptionsById[typeId];
                var obj = ((SpecializedTypeDescription<T>)description).DeserializeHandler(reader);
                return obj;
            }
            finally
            {
                ReaderPool.ReleaseFormatter(reader);
            }
        }

        public static T Deserialize<T>(BinaryReader reader)
        {
            var state = _state;
            var typeId = reader.ReadInt16();
            if (typeId >= state.DescriptionsById.Length)
                state.UnknownTypeId(typeId);

            var description = state.DescriptionsById[typeId];
            var obj = ((SpecializedTypeDescription<T>)description).DeserializeHandler(reader);
            return obj;
        }

        public static void Clear()
        {
            lock (Locker)
            {
                var state = new SerializerState();
                state.DescriptionsById = new TypeDescription[1];
                Interlocked.Exchange(ref _state, state);
            }
        }

        internal static TypeDescription GetTypeDescription(Type type, SerializerState state)
        {
            return GetTypeDescription(type, state.LastTypeId, state, true);
        }

        internal static TypeDescription GetTypeDescription(Type type, ushort typeId, SerializerState state, bool registerUnderlyingTypes)
        {
            state.LastTypeId = Math.Max((ushort)(typeId + 1), state.LastTypeId);
            if (type.IsBuiltInType())
            {
                var nullableUnderlyingType = Nullable.GetUnderlyingType(type);
                if (nullableUnderlyingType != null)
                {
                    if (!nullableUnderlyingType.IsBuiltInType() && registerUnderlyingTypes)
                    {
                        var td = GetTypeDescription(nullableUnderlyingType, state);
                        td?.Build();
                    }
                }
                return null;
            }

            TypeDescription description;
            if (state.AllTypeDescriptions.TryGetValue(type, out description))
            {
                description.TypeId = typeId;
                return description;
            }

            if (type.IsDictionaryType() && registerUnderlyingTypes)
            {
                var dictTypes = type.GetDictionaryArguments();
                var keyTd = GetTypeDescription(dictTypes.Item1, state);
                keyTd?.Build();
                var valTd = GetTypeDescription(dictTypes.Item2, state);
                valTd?.Build();
                return null;
            }

            if (type.IsEnumerableType() && registerUnderlyingTypes)
            {
                var elementType = type.GetEnumerableArgument();
                if (!elementType.GetTypeInfo().IsInterface && !elementType.GetTypeInfo().IsAbstract)
                {
                    var td = GetTypeDescription(elementType, state);
                    td?.Build();
                }
                return null;
            }

            if (type.IsAnonymousType())
                description = CreateTypeDescription(typeof(AnonymousTypeDescription<>), type, typeId, type, state);
            else
                description = CreateTypeDescription(typeof(ComplexTypeDescription<>), type, typeId, type, state);
            state.AddDescription(type, description);
            return description;
        }

        private static void RegisterPrimitiveTypes(SerializerState state)
        {
            var primitiveTypes = BuilderFactory.GetPrimitiveTypes().ToArray();
            for (ushort i = 0; i < primitiveTypes.Length; i++)
            {
                if (state.GetDescription(primitiveTypes[i]) == null)
                {
                    var description = CreateTypeDescription(typeof(SimpleTypeDescription<>), primitiveTypes[i], i, primitiveTypes[i], state);
                    state.AddDescription(primitiveTypes[i], description);
                    description.Build();
                }
            }
        }

        private static TypeDescription CreateTypeDescription(Type descriptionType, IEnumerable<Type> argTypes, params object[] constructorArgs)
        {
            try
            {
                return (TypeDescription)Activator.CreateInstance(descriptionType.MakeGenericType(argTypes.ToArray()), constructorArgs);
            }
            catch (TargetInvocationException e)
            {
                if (e.InnerException != null)
                    throw e.InnerException;
                else
                    throw;
            }
        }

        private static TypeDescription CreateTypeDescription(Type descriptionType, Type argType, params object[] constructorArgs)
        {
            return CreateTypeDescription(descriptionType, new[] { argType }, constructorArgs);
        }
    }
}
