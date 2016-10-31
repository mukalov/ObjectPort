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
    using System.Diagnostics;
    using System.IO;
    using System.Linq.Expressions;

    internal class CheckNullBuilder<T> : ActionProviderBuilder<T>
    {
        private readonly MemberSerializerBuilder _innerBuilder;

        public CheckNullBuilder(MemberSerializerBuilder innerBuilder)
        {
            _innerBuilder = innerBuilder;
        }

        public override Expression GetSerializerExpression(Type memberType, Expression getterExp, ParameterExpression writerExp)
        {
            Debug.Assert(_innerBuilder != null);
            if (memberType.IsValueType)
                return _innerBuilder.GetSerializerExpression(memberType, getterExp, writerExp);

            var boolType = typeof(bool);
            var valueExp = getterExp;
            var writeMethod = GetWriterMethod("Write", boolType);
            var bodyExp = _innerBuilder.GetSerializerExpression(memberType, getterExp, writerExp);
            return Expression.IfThenElse(
                Expression.Equal(valueExp, Expression.Constant(null, memberType)),
                Expression.Call(writerExp, writeMethod, Expression.Constant(false, boolType)),
                Expression.Block(Expression.Call(writerExp, writeMethod, Expression.Constant(true, boolType)), bodyExp));
        }

        public override Expression GetDeserializerExpression(Type memberType, ParameterExpression readerExpression)
        {
            Debug.Assert(_innerBuilder != null);
            if (memberType.IsValueType)
                return _innerBuilder.GetDeserializerExpression(memberType, readerExpression);

            var defaultValExp = memberType.IsValueType ?
                Expression.New(memberType) :
                (Expression)Expression.Constant(null, memberType);

            var readBool = typeof(BinaryReader).GetMethod("ReadBoolean");
            var readExp = Expression.Call(readerExpression, readBool);
            var castedMemberExp = Expression.TypeAs(_innerBuilder.GetDeserializerExpression(memberType, readerExpression), memberType);
            var conditionalExp = Expression.Condition(
                Expression.Equal(readExp, Expression.Constant(true, typeof(bool))),
                castedMemberExp,
                defaultValExp);
            return conditionalExp;
        }
    }
}
