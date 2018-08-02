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
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    internal abstract class PrimitiveBuilder<T> : ActionProviderBuilder<T>
    {
        public override Expression GetSerializerExpression(Type memberType, Expression getterExp, ParameterExpression writerExp)
        {
            var writeMethod = GetWriteMethod(memberType);
            if (writeMethod == null)
                this.TypeNotSupported(memberType);

            var valueToWriteExp = FromValue(getterExp);
            var writeParameters = GetWriteParameters(writerExp, valueToWriteExp);
            var writeExp = IsStaticWriter ?
                Expression.Call(writeMethod, writeParameters) :
                Expression.Call(writerExp, writeMethod, writeParameters);
            return writeExp;
        }

        public override Expression GetDeserializerExpression(Type memberType, ParameterExpression readerExpression)
        {
            var readMethod = GetReadMethod();
            var readParameters = GetReadParameters(readerExpression);
            var readExp = IsStaticReader && readParameters != null && readParameters.Any() ? 
                Expression.Call(readMethod, readParameters) : 
                Expression.Call(readerExpression, readMethod);
            return ToValue(readExp);
        }

        protected abstract MethodInfo GetReadMethod();

        protected virtual MethodInfo GetWriteMethod(Type type)
        {
            return GetWriterMethod("Write", type);
        }

        protected virtual Expression FromValue(Expression valueExp)
        {
            return valueExp;
        }

        protected virtual Expression ToValue(Expression readExp)
        {
            return readExp;
        }

        protected virtual bool IsStaticReader => false;

        protected virtual bool IsStaticWriter => false;

        protected virtual Expression[] GetReadParameters(Expression reader)
        {
            return new Expression[] { };
        }

        protected virtual Expression[] GetWriteParameters(Expression writer, Expression parameter)
        {
            return new[] { parameter }.ToArray(); 
        }
    }
}
