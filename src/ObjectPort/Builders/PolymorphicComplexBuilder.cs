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

namespace ObjectPort.Builders
{
    using Common;
    using Descriptions;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    internal class PolymorphicComplexBuilder<T> : MemberSerializerBuilder, ICompiledActionProvider<T>
        where T : class
    {
        internal class TypeDescriptionWithIndex
        {
            public TypeDescription Description;
            public byte Index;
        }

        private readonly AdaptiveHashtable<TypeDescriptionWithIndex> _typeDescriptionsByHashCode;
        private readonly TypeDescription[] _typeDescriptionsByIndex;

        public PolymorphicComplexBuilder(Type type, SerializerState state)
        {
            _typeDescriptionsByHashCode = new AdaptiveHashtable<TypeDescriptionWithIndex>();
            var typeDescriptions = new Dictionary<int, TypeDescriptionWithIndex>();
            foreach (var description in state.GetDescriptionsForDerivedTypes(type))
            {
                _typeDescriptionsByHashCode.AddValue(
                    (uint)RuntimeHelpers.GetHashCode(description.Type),
                    new TypeDescriptionWithIndex
                    {
                        Description = description
                    });

                if (!typeDescriptions.ContainsKey(description.Type.GetHashCode()))
                    typeDescriptions.Add(description.Type.GetHashCode(), new TypeDescriptionWithIndex
                    {
                        Description = description
                    });
            }
            _typeDescriptionsByIndex = new TypeDescription[typeDescriptions.Count];
            var index = (byte)0;
            foreach (var item in typeDescriptions)
            {
                item.Value.Index = index;
                var val = _typeDescriptionsByHashCode.TryGetValue((uint)RuntimeHelpers.GetHashCode(item.Value.Description.Type));
                Debug.Assert(val != null);
                val.Index = index;
                _typeDescriptionsByIndex[index++] = item.Value.Description;
            }
        }

        public override Expression GetSerializerExpression(Type memberType, Expression getterExp, ParameterExpression writerExp)
        {
            var valueExp = getterExp;
            var thisExp = Expression.Constant(this, GetType());
            return Expression.Call(thisExp, SerializeMethod, valueExp, writerExp);
        }

        public override Expression GetDeserializerExpression(Type memberType, ParameterExpression readerExp)
        {
            var thisExp = Expression.Constant(this, GetType());
            var valueExp = Expression.Call(thisExp, DeserializeMethod, readerExp);
            return Expression.TypeAs(valueExp, memberType);
        }

        public Action<T, BinaryWriter> GetSerializerAction(Type memberType, ParameterExpression valueExp, ParameterExpression writerExp)
        {
            return Expression.Lambda<Action<T, BinaryWriter>>(
                GetSerializerExpression(memberType, valueExp, writerExp), valueExp, writerExp).Compile();
        }

        public Func<BinaryReader, T> GetDeserializerAction(Type memberType, ParameterExpression readerExp)
        {
            return Expression.Lambda<Func<BinaryReader, T>>(
                GetDeserializerExpression(memberType, readerExp), readerExp).Compile();
        }

        internal void SerializeStruct(T obj, BinaryWriter writer)
        {
            var descrInfo = _typeDescriptionsByHashCode.GetValue((uint)RuntimeHelpers.GetHashCode(obj.GetType()));
            writer.Write(descrInfo.Index);
            descrInfo.Description.Serialize(writer, obj);
        }

        internal void SerializeClass(T obj, BinaryWriter writer)
        {
            if (obj == default(T))
                writer.Write(false);
            else
            {
                writer.Write(true);
                var descrInfo = _typeDescriptionsByHashCode.GetValue((uint)RuntimeHelpers.GetHashCode(obj.GetType()));
                writer.Write(descrInfo.Index);
                descrInfo.Description.Serialize(writer, obj);
            }
        }

        internal T DeserializeStruct(BinaryReader reader)
        {
            var index = reader.ReadByte();
            return (T)_typeDescriptionsByIndex[index].Deserialize(reader);
        }

        internal T DeserializeClass(BinaryReader reader)
        {
            var result = default(T);
            var isNotNull = reader.ReadBoolean();
            if (isNotNull)
            {
                var index = reader.ReadByte();
                result = (T)_typeDescriptionsByIndex[index].Deserialize(reader);
            }
            return result;
        }

        protected MethodInfo SerializeMethod
        {
            get
            {
                var methodName = "SerializeClass";
                return GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
            }
        }

        protected MethodInfo DeserializeMethod
        {
            get
            {
                var methodName = "DeserializeClass";
                return GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
            }
        }
    }
}
