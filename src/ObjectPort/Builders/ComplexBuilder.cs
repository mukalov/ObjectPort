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
    using Descriptions;
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using Common;

    internal class ComplexBuilder<T> : ActionProviderBuilder<T>
    {
        private readonly TypeDescription _typeDescription;

        public ComplexBuilder(TypeDescription typeDescription)
        {
            _typeDescription = typeDescription;
        }

        public override Expression GetSerializerExpression(Type memberType, Expression getterExp, ParameterExpression writerExp)
        {
            var tdExp = Expression.Constant(_typeDescription, typeof(TypeDescription));
            var valueExp = memberType.GetTypeInfo().IsValueType ? Expression.Convert(getterExp, typeof(object)) : getterExp;
            var serializeSubTypeExp = Expression.Call(
                tdExp,
                typeof(TypeDescription).GetTypeInfo().GetMethod("Serialize", BindingFlags.NonPublic | BindingFlags.Instance),
                writerExp,
                valueExp);
            return serializeSubTypeExp;
        }

        public override Expression GetDeserializerExpression(Type memberType, ParameterExpression readerExpression)
        {
            return _typeDescription.GetDeserializerExpression(readerExpression);
        }
    }
}
