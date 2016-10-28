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
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    internal class RegularEnumerableBuilder<T> : EnumerableBuilder<T>
    {
        private readonly MemberSerializerBuilder _elementBuilder;
        private Action<T, BinaryWriter> _elementSerializer;
        private Func<BinaryReader, T> _elementDeserializer;

        public RegularEnumerableBuilder(Type enumerableType, Type baseElementType, SerializerState state)
            : base(enumerableType, baseElementType)
        {
            _elementBuilder = BuilderFactory.GetBuilder(baseElementType, null, state);
        }

        public override Expression GetSerializerExpression(Type memberType, Expression getterExp, ParameterExpression writerExp)
        {
            var elementExp = Expression.Parameter(BaseElementType, "element");
            _elementSerializer = ((ICompiledActionProvider<T>)_elementBuilder).GetSerializerAction(BaseElementType, elementExp, writerExp);
            var valueExp = getterExp;
            var thisExp = Expression.Constant(this, BuilderSpecificType);
            return Expression.Call(thisExp, SerializeMethod, valueExp, writerExp);
        }

        public override Expression GetDeserializerExpression(Type memberType, ParameterExpression readerExp)
        {
            _elementDeserializer = ((ICompiledActionProvider<T>)_elementBuilder).GetDeserializerAction(BaseElementType, readerExp);
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
                _elementSerializer(item, writer);
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
                _elementSerializer(item, writer);
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
                result[i] = _elementDeserializer(reader);
            return constructorIndex == ArrayConstructorIndex ? result : ConstructorsByIndex[constructorIndex](result);
        }

        protected MethodInfo SerializeMethod
        {
            get
            {
                return SerializeAsArray ?
                    BuilderSpecificType.GetMethod("SerializeArray", BindingFlags.NonPublic | BindingFlags.Instance) :
                    BuilderSpecificType.GetMethod("SerializeEnumerable", BindingFlags.NonPublic | BindingFlags.Instance);
            }
        }

        protected MethodInfo DeserializeMethod
        {
            get
            {
                return BuilderSpecificType.GetMethod("Deserialize", BindingFlags.NonPublic | BindingFlags.Instance);
            }
        }
    }
}
