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
    using Builders;
    using Common;
    using System;
    using System.Linq.Expressions;
    using System.Reflection;

    internal abstract class MemberDescription
    {
        private readonly Lazy<MemberSerializerBuilder> _serializerBuilder;
        public Action<object, Writer> Serializer;
        public Expression SerializerExpression;

        public MemberInfo MemberInfo { get; }
        public string Name { get; private set; }
        public Type Type { get; protected set; }
        public TypeDescription NestedTypeDescription { get; set; }

        internal MemberDescription(MemberInfo memberInfo, SerializerState state)
        {
            _serializerBuilder = new Lazy<MemberSerializerBuilder>(() => BuilderFactory.GetBuilder(Type, NestedTypeDescription, state));
            MemberInfo = memberInfo;
            Name = MemberInfo.Name;
        }

        public MemberAssignment GetAssignment(Expression valueExp)
        {
            return Expression.Bind(MemberInfo, valueExp);
        }

        public void CompileSerializer(ParameterExpression instanceExp, ParameterExpression writerExp)
        {
            SerializerExpression = _serializerBuilder.Value.GetSerializerExpression(Type, GetterExpression(instanceExp), writerExp);
        }

        public abstract Expression GetterExpression(Expression instanceExp);

        public Expression DeserializeExpression(ParameterExpression readerExpression)
        {
            return _serializerBuilder.Value.GetDeserializerExpression(Type, readerExpression);
        }
    }
}
