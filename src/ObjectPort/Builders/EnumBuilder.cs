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
    using System.Linq.Expressions;

    internal class EnumBuilder<T> : ActionProviderBuilder<T>
    {
        private readonly MemberSerializerBuilder _baseBuilder; 
        private readonly Type _enumType;
        private readonly Type _enumBaseType;

        public EnumBuilder(Type enumType, Type enumBaseType)
        {
            _enumType = enumType;
            _enumBaseType = enumBaseType;
            _baseBuilder = BuilderFactory.GetBuilder(_enumBaseType, null, null);
        }

        public override Expression GetSerializerExpression(Type memberType, Expression getterExp, ParameterExpression writerExp)
        {
            return _baseBuilder.GetSerializerExpression(_enumBaseType, Expression.Convert(getterExp, _enumBaseType), writerExp);
        }

        public override Expression GetDeserializerExpression(Type memberType, ParameterExpression readerExp)
        {
            return Expression.Convert(_baseBuilder.GetDeserializerExpression(_enumBaseType, readerExp), _enumType);
        }
    }
}
