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
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    internal abstract class DictionaryBuilder<TKey, TVal> : MemberSerializerBuilder
    {
        internal struct Constructor
        {
            public Func<IDictionary<TKey, TVal>, IDictionary<TKey, TVal>> Method;
            public ushort Index;
        }

        private readonly Type _dictionaryType;
        private readonly bool _isIDictionary;

        protected const int NullLength = -1;
        protected Func<IDictionary<TKey, TVal>, IDictionary<TKey, TVal>>[] ConstructorsByIndex;
        protected AdaptiveHashtable<Constructor> ConstructorsByType;
        protected const ushort ArrayConstructorIndex = ushort.MaxValue;
        protected Type BuilderSpecificType;
        protected Type KeyType;
        protected Type BaseValType;

        internal DictionaryBuilder(Type dictionaryType, Type keyType, Type baseValType)
        {
            KeyType = keyType;
            BaseValType = baseValType;
            BuilderSpecificType = GetType();
            if (dictionaryType.IsGenericType && dictionaryType.GetGenericTypeDefinition() == typeof(IDictionary<,>))
            {
                _dictionaryType = typeof(Dictionary<,>).MakeGenericType(keyType, baseValType);
                _isIDictionary = true;
            }
            else
            {
                _dictionaryType = dictionaryType;
                _isIDictionary = false;
            }

            var dictionarySpecificType = typeof(IDictionary<,>).MakeGenericType(keyType, baseValType);
            var argExp = Expression.Parameter(dictionarySpecificType);
            Func<Type, Expression> getConstructorExp = type => Expression.New(type.GetConstructor(new[] { dictionarySpecificType }), argExp);

            var enumerableTypes = new Dictionary<Type, Func<Type, Expression>>
            {
                [typeof(Dictionary<,>).MakeGenericType(keyType, baseValType)] = getConstructorExp,
                [typeof(SortedList<,>).MakeGenericType(keyType, baseValType)] = getConstructorExp,
                [typeof(SortedDictionary<,>).MakeGenericType(keyType, baseValType)] = getConstructorExp
            };

            ConstructorsByIndex = new Func<IDictionary<TKey, TVal>, IDictionary<TKey, TVal>>[enumerableTypes.Count()];
            ConstructorsByType = new AdaptiveHashtable<Constructor>();
            var index = (ushort)0;
            foreach (var item in enumerableTypes)
            {
                var specificType = item.Key;
                var constructorExp = item.Value(specificType);
                var method = Expression.Lambda<Func<IDictionary<TKey, TVal>, IDictionary<TKey, TVal>>>(constructorExp, argExp).Compile();
                ConstructorsByIndex[index] = method;
                ConstructorsByType.AddValue(
                    (uint)RuntimeHelpers.GetHashCode(specificType),
                    new Constructor
                    {
                        Index = index++,
                        Method = method
                    });
            }
        }

        protected MethodInfo SerializeMethod
        {
            get
            {
                return BuilderSpecificType.GetMethod("Serialize", BindingFlags.NonPublic | BindingFlags.Instance);
            }
        }

        protected MethodInfo DeserializeMethod
        {
            get
            {
                return BuilderSpecificType.GetMethod("Deserialize", BindingFlags.NonPublic | BindingFlags.Instance);
            }
        }

        protected Type DeserializedType
        {
            get
            {
                return _dictionaryType;
            }
        }
    }
}
