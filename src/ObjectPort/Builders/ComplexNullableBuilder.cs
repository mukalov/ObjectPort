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
    using System.IO;
    using System.Linq.Expressions;
    using System.Reflection;

    internal class ComplexNullableBuilder<T> : MemberSerializerBuilder, ICompiledActionProvider<T?>
        where T : struct
    {
        private readonly Type _underlyingType;
        private readonly TypeDescription _description;
        private readonly Type _builderType;

        public ComplexNullableBuilder(Type underlyingType, SerializerState state)
        {
            _underlyingType = underlyingType;
            _description = state.GetDescription(underlyingType);
            _builderType = GetType();
        }

        public override Expression GetSerializerExpression(Type memberType, Expression getterExp, ParameterExpression writerExp)
        {
            var serializeMethod = _builderType.GetMethod("Serialize", BindingFlags.NonPublic | BindingFlags.Instance);
            var valueExp = getterExp;
            var thisExp = Expression.Constant(this, _builderType);
            return Expression.Call(thisExp, serializeMethod, valueExp, writerExp);
        }

        public override Expression GetDeserializerExpression(Type memberType, ParameterExpression readerExp)
        {
            var deserializeMethod = _builderType.GetMethod("Deserialize", BindingFlags.NonPublic | BindingFlags.Instance);
            var thisExp = Expression.Constant(this, _builderType);
            return Expression.Call(thisExp, deserializeMethod, readerExp);
        }

        public Action<T?, BinaryWriter> GetSerializerAction(Type memberType, ParameterExpression valueExp, ParameterExpression writerExp)
        {
            return Expression.Lambda<Action<T?, BinaryWriter>>(
                GetSerializerExpression(memberType, valueExp, writerExp), valueExp, writerExp).Compile();
        }

        public Func<BinaryReader, T?> GetDeserializerAction(Type memberType, ParameterExpression readerExp)
        {
            return Expression.Lambda<Func<BinaryReader, T?>>(
                GetDeserializerExpression(memberType, readerExp), readerExp).Compile();
        }

        internal void Serialize(T? nullable, BinaryWriter writer)
        {
            if (!nullable.HasValue)
            {
                writer.Write(false);
                return;
            }
            writer.Write(true);
            _description.Serialize(writer, nullable.Value);
        }

        internal T? Deserialize(BinaryReader reader)
        {
            var notNull = reader.ReadBoolean();
            if (!notNull)
                return null;
            return (T)_description.Deserialize(reader);
        }
    }
}
