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
    using System.Linq;
    using System.Linq.Expressions;
    using System.Runtime.CompilerServices;
    using System.Reflection;

    internal class PolymorphicEnumerableBuilder<T> : EnumerableBuilder<T>
    {
        private class TypeDescriptionWithIndex
        {
            public TypeDescription Description;
            public byte Index;
        }

        private readonly AdaptiveHashtable<TypeDescriptionWithIndex> _elementTypeDescriptionsByHashCode;
        private readonly TypeDescription[] _elementTypeDescriptionsByIndex;

        public PolymorphicEnumerableBuilder(Type enumerableType, Type baseElementType, SerializerState state)
            : base(enumerableType, baseElementType)
        {
            _elementTypeDescriptionsByHashCode = new AdaptiveHashtable<TypeDescriptionWithIndex>();
            var elementTypeDescriptions = new Dictionary<int, TypeDescriptionWithIndex>();
            foreach (var description in state.AllTypeDescriptions.Select(i => i.Value).ToArray())
            {
                if (description.Type == BaseElementType
                    || description.Type.IsAssignableFrom(BaseElementType)
                    || description.Type.IsSubclassOf(BaseElementType))
                {
                    _elementTypeDescriptionsByHashCode.AddValue(
                        (uint)RuntimeHelpers.GetHashCode(description.Type),
                        new TypeDescriptionWithIndex
                        {
                            Description = description
                        });

                    if (!elementTypeDescriptions.ContainsKey(description.Type.GetHashCode()))
                        elementTypeDescriptions.Add(description.Type.GetHashCode(), new TypeDescriptionWithIndex
                        {
                            Description = description
                        });
                }
            }
            _elementTypeDescriptionsByIndex = new TypeDescription[elementTypeDescriptions.Count];
            var index = (byte)0;
            foreach (var item in elementTypeDescriptions)
            {
                item.Value.Index = index;
                var val = _elementTypeDescriptionsByHashCode.GetValue((uint)RuntimeHelpers.GetHashCode(item.Value.Description.Type));
                Debug.Assert(val != null);
                val.Index = index;
                _elementTypeDescriptionsByIndex[index++] = item.Value.Description;
            }
        }

        public override Expression GetSerializerExpression(Type memberType, Expression getterExp, ParameterExpression writerExp)
        {
            var valueExp = getterExp;
            var thisExp = Expression.Constant(this, BuilderSpecificType);
            return Expression.Call(thisExp, SerializeMethod, valueExp, writerExp);
        }

        public override Expression GetDeserializerExpression(Type memberType, ParameterExpression readerExp)
        {
            var thisExp = Expression.Constant(this, BuilderSpecificType);
            var valueExp = Expression.Call(thisExp, DeserializeMethod, readerExp);
            return Expression.TypeAs(valueExp, DeserializedType);
        }

        internal void SerializeEnumerable(IEnumerable<T> enumerable, BinaryWriter writer)
        {
            if (enumerable == null)
            {
                writer.Write(NullLength);
                return;
            }

            writer.Write(enumerable.Count());
            var constructorIndex = ConstructorsByType.GetValue((uint)RuntimeHelpers.GetHashCode(enumerable.GetType())).Index;
            writer.Write(constructorIndex);
            foreach (var item in enumerable)
            {
                if (EqualityComparer<T>.Default.Equals(item, default(T)))
                    writer.Write(false);
                else
                {
                    writer.Write(true);
                    var descrInfo = _elementTypeDescriptionsByHashCode.GetValue((uint)RuntimeHelpers.GetHashCode(item.GetType()));
                    writer.Write(descrInfo.Index);
                    descrInfo.Description.Serialize(writer, item);
                }
            }
        }

        internal void SerializeEnumerableOfStruct(IEnumerable<T> enumerable, BinaryWriter writer)
        {
            if (enumerable == null)
            {
                writer.Write(NullLength);
                return;
            }

            writer.Write(enumerable.Count());
            var constructorIndex = ConstructorsByType.GetValue((uint)RuntimeHelpers.GetHashCode(enumerable.GetType())).Index;
            writer.Write(constructorIndex);
            foreach (var item in enumerable)
            {
                var descrInfo = _elementTypeDescriptionsByHashCode.GetValue((uint)RuntimeHelpers.GetHashCode(item.GetType()));
                writer.Write(descrInfo.Index);
                descrInfo.Description.Serialize(writer, item);
            }
        }

        internal void SerializeArray(IEnumerable<T> enumerable, BinaryWriter writer)
        {
            if (enumerable == null)
            {
                writer.Write(NullLength);
                return;
            }

            var array = enumerable as T[] ?? enumerable.ToArray();
            writer.Write(array.Length);
            writer.Write(ArrayConstructorIndex);
            for (var i = 0; i < array.Length; i++)
            {
                var item = array[i];
                if (EqualityComparer<T>.Default.Equals(item, default(T)))
                    writer.Write(false);
                else
                {
                    writer.Write(true);
                    var descrInfo = _elementTypeDescriptionsByHashCode.GetValue((uint)RuntimeHelpers.GetHashCode(item.GetType()));
                    writer.Write(descrInfo.Index);
                    descrInfo.Description.Serialize(writer, item);
                }
            }
        }

        internal void SerializeArrayOfStruct(IEnumerable<T> enumerable, BinaryWriter writer)
        {
            if (enumerable == null)
            {
                writer.Write(NullLength);
                return;
            }

            var array = enumerable as T[] ?? enumerable.ToArray();
            writer.Write(array.Length);
            writer.Write(ArrayConstructorIndex);
            for (var i = 0; i < array.Length; i++)
            {
                var item = array[i];
                var descrInfo = _elementTypeDescriptionsByHashCode.GetValue((uint)RuntimeHelpers.GetHashCode(item.GetType()));
                writer.Write(descrInfo.Index);
                descrInfo.Description.Serialize(writer, item);
            }
        }

        internal IEnumerable<T> Deserialize(BinaryReader reader)
        {
            var length = reader.ReadInt32();
            if (length == NullLength)
                return null;

            var constructorIndex = reader.ReadUInt16();
            var result = new T[length];
            for (var i = 0; i < length; i++)
            {
                var isNotNull = reader.ReadBoolean();
                if (isNotNull)
                {
                    var index = reader.ReadByte();
                    result[i] = (T)_elementTypeDescriptionsByIndex[index].Deserialize(reader);
                }
                else
                    result[i] = default(T);
            }
            return constructorIndex == ArrayConstructorIndex ? result : ConstructorsByIndex[constructorIndex](result);
        }

        internal IEnumerable<T> DeserializeEnumerableOfStruct(BinaryReader reader)
        {
            var length = reader.ReadInt32();
            if (length == NullLength)
                return null;

            var constructorIndex = reader.ReadUInt16();
            var result = new T[length];
            for (var i = 0; i < length; i++)
            {
                var index = reader.ReadByte();
                result[i] = (T)_elementTypeDescriptionsByIndex[index].Deserialize(reader);
            }
            return constructorIndex == ArrayConstructorIndex ? result : ConstructorsByIndex[constructorIndex](result);
        }

        protected MethodInfo SerializeMethod
        {
            get
            {
                var methodName = string.Empty;
                if (SerializeAsArray)
                    methodName = BaseElementType.IsValueType ? "SerializeArrayOfStruct" : "SerializeArray";
                else
                    methodName = BaseElementType.IsValueType ? "SerializeEnumerableOfStruct" : "SerializeEnumerable";
                return BuilderSpecificType.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
            }
        }

        protected MethodInfo DeserializeMethod
        {
            get
            {
                var methodName = BaseElementType.IsValueType ? "DeserializeEnumerableOfStruct" : "Deserialize";
                return BuilderSpecificType.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
            }
        }
    }
}
