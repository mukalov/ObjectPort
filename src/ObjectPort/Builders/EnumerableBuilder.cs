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
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    internal class EnumerableBuilder<EnumerableT, T> : ActionProviderBuilder<EnumerableT>
    {
        internal struct Constructor
        {
            public Func<IEnumerable<T>, IEnumerable<T>> Method;
            public ushort Index;
        }

        protected const int NullLength = -1;
        protected const ushort ArrayConstructorIndex = ushort.MaxValue;

        private readonly Type _enumerableType;
        private readonly bool _isIEnumerable;
        private readonly MemberSerializerBuilder _elementBuilder;
        private readonly Func<IEnumerable<T>, IEnumerable<T>>[] _constructorsByIndex;
        private readonly AdaptiveHashtable<Constructor> _constructorsByType;
        private readonly Type _builderSpecificType;
        private readonly Type _baseElementType;
        private Action<T, BinaryWriter> _elementSerializer;
        private Func<BinaryReader, T> _elementDeserializer;

        protected bool SerializeAsArray
        {
            get
            {
                return _enumerableType.IsArray && !_isIEnumerable;
            }
        }

        protected Type DeserializedType
        {
            get
            {
                if (_enumerableType.IsArray)
                {
                    if (_isIEnumerable)
                        return typeof(IEnumerable<>).MakeGenericType(_baseElementType);
                    else
                        return _baseElementType.MakeArrayType();
                }
                else
                    return _enumerableType;
            }
        }

        public EnumerableBuilder(Type enumerableType, Type baseElementType, TypeDescription elementTypeDescription, SerializerState state)
        {
            _baseElementType = baseElementType;
            _builderSpecificType = GetType();
            if (enumerableType.GetTypeInfo().IsGenericType && enumerableType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                _enumerableType = baseElementType.MakeArrayType();
                _isIEnumerable = true;
            }
            else
            {
                _enumerableType = enumerableType;
                _isIEnumerable = false;
            }

            var ienumerableSpecificType = typeof(IEnumerable<>).MakeGenericType(baseElementType);
            var argExp = Expression.Parameter(ienumerableSpecificType);
            Func<Type, Expression> getConstructorExp = type => Expression.New(type.GetTypeInfo().GetConstructor(new[] { ienumerableSpecificType }), argExp);

            var enumerableTypes = new Dictionary<Type, Func<Type, Expression>>
            {
                [baseElementType.MakeArrayType()] = type => argExp,
                [typeof(List<>).MakeGenericType(baseElementType)] = getConstructorExp,
                [typeof(HashSet<>).MakeGenericType(baseElementType)] = getConstructorExp,
                [typeof(LinkedList<>).MakeGenericType(baseElementType)] = getConstructorExp,
                [typeof(Queue<>).MakeGenericType(baseElementType)] = getConstructorExp,
                [typeof(SortedSet<>).MakeGenericType(baseElementType)] = getConstructorExp,
                [typeof(Stack<>).MakeGenericType(baseElementType)] = getConstructorExp
            };

            _constructorsByIndex = new Func<IEnumerable<T>, IEnumerable<T>>[enumerableTypes.Count()];
            _constructorsByType = new AdaptiveHashtable<Constructor>();
            var index = (ushort)0;
            foreach (var item in enumerableTypes)
            {
                var specificType = item.Key;
                var constructorExp = item.Value(specificType);
                var method = Expression.Lambda<Func<IEnumerable<T>, IEnumerable<T>>>(constructorExp, argExp).Compile();
                _constructorsByIndex[index] = method;
                _constructorsByType.AddValue(
                    (uint)RuntimeHelpers.GetHashCode(specificType),
                    new Constructor
                    {
                        Index = index++,
                        Method = method
                    });
            }
            _elementBuilder = BuilderFactory.GetBuilder(baseElementType, elementTypeDescription, state);
        }

        public override Expression GetSerializerExpression(Type memberType, Expression getterExp, ParameterExpression writerExp)
        {
            var elementExp = Expression.Parameter(_baseElementType, "element");
            _elementSerializer = ((ICompiledActionProvider<T>)_elementBuilder).GetSerializerAction(_baseElementType, elementExp, writerExp);
            var valueExp = getterExp;
            var thisExp = Expression.Constant(this, _builderSpecificType);
            return Expression.Call(thisExp, SerializeMethod, valueExp, writerExp);
        }

        public override Expression GetDeserializerExpression(Type memberType, ParameterExpression readerExp)
        {
            _elementDeserializer = ((ICompiledActionProvider<T>)_elementBuilder).GetDeserializerAction(_baseElementType, readerExp);
            var thisExp = Expression.Constant(this, _builderSpecificType);
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
            var constructorIndex = _constructorsByType.TryGetValue((uint)RuntimeHelpers.GetHashCode(enumerable.GetType())).Index;
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
            return constructorIndex == ArrayConstructorIndex ? result : _constructorsByIndex[constructorIndex](result);
        }

        protected MethodInfo SerializeMethod
        {
            get
            {
                return SerializeAsArray ?
                    _builderSpecificType.GetTypeInfo().GetMethod("SerializeArray", BindingFlags.NonPublic | BindingFlags.Instance) :
                    _builderSpecificType.GetTypeInfo().GetMethod("SerializeEnumerable", BindingFlags.NonPublic | BindingFlags.Instance);
            }
        }

        protected MethodInfo DeserializeMethod
        {
            get
            {
                return _builderSpecificType.GetTypeInfo().GetMethod("Deserialize", BindingFlags.NonPublic | BindingFlags.Instance);
            }
        }
    }
}
