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
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    internal class DictionaryBuilder<TKey, TVal> : MemberSerializerBuilder
    {
        internal struct Constructor
        {
            public Func<IDictionary<TKey, TVal>, IDictionary<TKey, TVal>> Method;
            public ushort Index;
        }

        private const int NullLength = -1;
        private const ushort ArrayConstructorIndex = ushort.MaxValue;

        private readonly Type _dictionaryType;
        private readonly bool _isIDictionary;
        private readonly Func<IDictionary<TKey, TVal>, IDictionary<TKey, TVal>>[] _constructorsByIndex;
        private readonly AdaptiveHashtable<Constructor> _constructorsByType;
        private readonly Type _builderSpecificType;
        private readonly Type _keyType;
        private readonly Type _valType;
        private readonly MemberSerializerBuilder _keyBuilder;
        private readonly MemberSerializerBuilder _valBuilder;
        private Action<TKey, BinaryWriter> _keySerializer;
        private Action<TVal, BinaryWriter> _valSerializer;
        private Func<BinaryReader, TKey> _keyDeserializer;
        private Func<BinaryReader, TVal> _valDeserializer;

        protected MethodInfo SerializeMethod
        {
            get
            {
                return _builderSpecificType.GetMethod("Serialize", BindingFlags.NonPublic | BindingFlags.Instance);
            }
        }

        protected MethodInfo DeserializeMethod
        {
            get
            {
                return _builderSpecificType.GetMethod("Deserialize", BindingFlags.NonPublic | BindingFlags.Instance);
            }
        }

        protected Type DeserializedType
        {
            get
            {
                return _dictionaryType;
            }
        }

        public DictionaryBuilder(Type dictionaryType, Type keyType, Type valType, SerializerState state)
        {
            _keyType = keyType;
            _valType = valType;
            _builderSpecificType = GetType();
            if (dictionaryType.IsGenericType && dictionaryType.GetGenericTypeDefinition() == typeof(IDictionary<,>))
            {
                _dictionaryType = typeof(Dictionary<,>).MakeGenericType(keyType, valType);
                _isIDictionary = true;
            }
            else
            {
                _dictionaryType = dictionaryType;
                _isIDictionary = false;
            }

            var dictionarySpecificType = typeof(IDictionary<,>).MakeGenericType(keyType, valType);
            var argExp = Expression.Parameter(dictionarySpecificType);
            Func<Type, Expression> getConstructorExp = type => Expression.New(type.GetConstructor(new[] { dictionarySpecificType }), argExp);

            var enumerableTypes = new Dictionary<Type, Func<Type, Expression>>
            {
                [typeof(Dictionary<,>).MakeGenericType(keyType, valType)] = getConstructorExp,
                [typeof(SortedList<,>).MakeGenericType(keyType, valType)] = getConstructorExp,
                [typeof(SortedDictionary<,>).MakeGenericType(keyType, valType)] = getConstructorExp
            };

            _constructorsByIndex = new Func<IDictionary<TKey, TVal>, IDictionary<TKey, TVal>>[enumerableTypes.Count()];
            _constructorsByType = new AdaptiveHashtable<Constructor>();
            var index = (ushort)0;
            foreach (var item in enumerableTypes)
            {
                var specificType = item.Key;
                var constructorExp = item.Value(specificType);
                var method = Expression.Lambda<Func<IDictionary<TKey, TVal>, IDictionary<TKey, TVal>>>(constructorExp, argExp).Compile();
                _constructorsByIndex[index] = method;
                _constructorsByType.AddValue(
                    (uint)RuntimeHelpers.GetHashCode(specificType),
                    new Constructor
                    {
                        Index = index++,
                        Method = method
                    });
            }
            _keyBuilder = BuilderFactory.GetBuilder(keyType, null, state);
            _valBuilder = BuilderFactory.GetBuilder(valType, null, state);
        }

        public override Expression GetSerializerExpression(Type memberType, Expression getterExp, ParameterExpression writerExp)
        {
            var keyExp = Expression.Parameter(_keyType, "key");
            _keySerializer = ((ICompiledActionProvider<TKey>)_keyBuilder).GetSerializerAction(_keyType, keyExp, writerExp);
            var valExp = Expression.Parameter(_valType, "val");
            _valSerializer = ((ICompiledActionProvider<TVal>)_valBuilder).GetSerializerAction(_valType, valExp, writerExp);

            var valueExp = getterExp;
            var thisExp = Expression.Constant(this, _builderSpecificType);
            return Expression.Call(thisExp, SerializeMethod, valueExp, writerExp);
        }

        public override Expression GetDeserializerExpression(Type memberType, ParameterExpression readerExp)
        {
            _keyDeserializer = ((ICompiledActionProvider<TKey>)_keyBuilder).GetDeserializerAction(_keyType, readerExp);
            _valDeserializer = ((ICompiledActionProvider<TVal>)_valBuilder).GetDeserializerAction(_valType, readerExp);
            var thisExp = Expression.Constant(this, _builderSpecificType);
            var valueExp = Expression.Call(thisExp, DeserializeMethod, readerExp);
            return Expression.TypeAs(valueExp, DeserializedType);
        }

        internal void Serialize(IDictionary<TKey, TVal> dictionary, BinaryWriter writer)
        {
            if (dictionary == null)
            {
                writer.Write(NullLength);
                return;
            }

            writer.Write(dictionary.Count());
            var constructorIndex = _constructorsByType.TryGetValue((uint)RuntimeHelpers.GetHashCode(dictionary.GetType())).Index;
            writer.Write(constructorIndex);
            foreach (var item in dictionary)
            {
                _keySerializer(item.Key, writer);
                _valSerializer(item.Value, writer);
            }
        }

        internal IDictionary<TKey, TVal> Deserialize(BinaryReader reader)
        {
            var length = reader.ReadInt32();
            if (length == NullLength)
                return null;

            var constructorIndex = reader.ReadUInt16();
            var result = new Dictionary<TKey, TVal>(length);
            for (var i = 0; i < length; i++)
            {
                var key = _keyDeserializer(reader);
                var val = _valDeserializer(reader);
                result[key] = val;
            }
            return _constructorsByIndex[constructorIndex](result);
        }
    }
}
