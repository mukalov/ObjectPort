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
    using ObjectPort.Common;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.CompilerServices;


    internal abstract class EnumerableBuilder<T> : MemberSerializerBuilder
    {
        internal struct Constructor
        {
            public Func<IEnumerable<T>, IEnumerable<T>> Method;
            public ushort Index;
        }

        private readonly Type _enumerableType;
        private readonly bool _isIEnumerable;

        protected const int NullLength = -1;
        protected Func<IEnumerable<T>, IEnumerable<T>>[] ConstructorsByIndex;
        protected AdaptiveHashtable<Constructor> ConstructorsByType;
        protected const ushort ArrayConstructorIndex = ushort.MaxValue;
        protected Type BuilderSpecificType;
        protected Type BaseElementType;

        internal EnumerableBuilder(Type enumerableType, Type baseElementType)
        {
            BaseElementType = baseElementType;
            BuilderSpecificType = GetType();
            if (enumerableType.IsGenericType && enumerableType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
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
            Func<Type, Expression> getConstructorExp = type => Expression.New(type.GetConstructor(new[] { ienumerableSpecificType }), argExp);

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

            ConstructorsByIndex = new Func<IEnumerable<T>, IEnumerable<T>>[enumerableTypes.Count()];
            ConstructorsByType = new AdaptiveHashtable<Constructor>();
            var index = (ushort)0;
            foreach (var item in enumerableTypes)
            {
                var specificType = item.Key;
                var constructorExp = item.Value(specificType);
                var method = Expression.Lambda<Func<IEnumerable<T>, IEnumerable<T>>>(constructorExp, argExp).Compile();
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
                return _enumerableType.IsArray && !_isIEnumerable ?
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

        protected Type DeserializedType
        {
            get
            {
                if (_enumerableType.IsArray)
                {
                    if (_isIEnumerable)
                        return typeof(IEnumerable<>).MakeGenericType(BaseElementType);
                    else
                        return BaseElementType.MakeArrayType();
                }
                else
                    return _enumerableType;
            }
        }
    }
}
