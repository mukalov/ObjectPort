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

namespace ObjectPort.Descriptions
{
    using Common;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    internal class AnonymousTypeDescription<T> : ComplexTypeDescription<T>
    {
        public AnonymousTypeDescription(ushort typeId, Type type, SerializerState state) 
            : base(typeId, type, state)
        {
        }

        internal override Expression GetDeserializerExpression(ParameterExpression readerExpression)
        {
            var anonTypeArgsExpressions = new List<Expression>();
            foreach (var description in Descriptions)
                anonTypeArgsExpressions.Add(description.DeserializeExpression(readerExpression));
            return Expression.New(Type.GetTypeInfo().GetConstructor(Descriptions.Select(p => p.Type).ToArray()), anonTypeArgsExpressions);
        }

        internal override MemberDescription[] GetDescriptions(SerializerState state)
        {
            var piMap = Type.GetTypeInfo().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .ToDictionary(pi => pi.Name);
            return Type.GetTypeInfo().GetConstructors()[0]
                .GetParameters()
                .Select(p => new PropertyDescription(piMap[p.Name], state)
                {
                    NestedTypeDescription = Serializer.GetTypeDescription(p.ParameterType, state)
                }).ToArray();
        }
    }
}
