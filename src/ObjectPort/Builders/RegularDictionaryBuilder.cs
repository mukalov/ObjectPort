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
    using System.Runtime.CompilerServices;

    internal class RegularDictionaryBuilder<TKey, TVal> : DictionaryBuilder<TKey, TVal>
    {
        private readonly MemberSerializerBuilder _keyBuilder;
        private readonly MemberSerializerBuilder _valBuilder;
        private Action<TKey, BinaryWriter> _keySerializer;
        private Action<TVal, BinaryWriter> _valSerializer;
        private Func<BinaryReader, TKey> _keyDeserializer;
        private Func<BinaryReader, TVal> _valDeserializer;

        public RegularDictionaryBuilder(Type dictionaryType, Type keyType, Type valType, SerializerState state)
            : base(dictionaryType, keyType, valType)
        {
            _keyBuilder = BuilderFactory.GetBuilder(keyType, null, state);
            _valBuilder = BuilderFactory.GetBuilder(valType, null, state);
        }

        public override Expression GetSerializerExpression(Type memberType, Expression getterExp, ParameterExpression writerExp)
        {
            var keyExp = Expression.Parameter(KeyType, "key");
            _keySerializer = ((ICompiledActionProvider<TKey>)_keyBuilder).GetSerializerAction(KeyType, keyExp, writerExp);
            var valExp = Expression.Parameter(BaseValType, "val");
            _valSerializer = ((ICompiledActionProvider<TVal>)_valBuilder).GetSerializerAction(BaseValType, valExp, writerExp);

            var valueExp = getterExp;
            var thisExp = Expression.Constant(this, BuilderSpecificType);
            return Expression.Call(thisExp, SerializeMethod, valueExp, writerExp);
        }

        public override Expression GetDeserializerExpression(Type memberType, ParameterExpression readerExp)
        {
            _keyDeserializer = ((ICompiledActionProvider<TKey>)_keyBuilder).GetDeserializerAction(KeyType, readerExp);
            _valDeserializer = ((ICompiledActionProvider<TVal>)_valBuilder).GetDeserializerAction(BaseValType, readerExp);
            var thisExp = Expression.Constant(this, BuilderSpecificType);
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
            var constructorIndex = ConstructorsByType.GetValue((uint)RuntimeHelpers.GetHashCode(dictionary.GetType())).Index;
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
            return ConstructorsByIndex[constructorIndex](result);
        }
    }
}
