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
    using System.Linq.Expressions;
    using System.Reflection;

    internal abstract class TypeDescription
    {
        private readonly Lazy<bool> _isInitialized;
        public Type Type { get; }
        public ushort TypeId { get; private set; }

        internal TypeDescription(ushort typeId, Type type, SerializerState state)
        {
            TypeId = typeId;
            Type = type;
            _isInitialized = new Lazy<bool>(() =>
            {
                InitDescriptions(state);
                InitSerializers();
                InitDeserializer();
                return true;
            });
        }

        internal void Build()
        {
            var isInitialized = _isInitialized.Value;
        }

        internal virtual void InitDescriptions(SerializerState state)
        {
        }

        internal abstract Expression GetDeserializerExpression(ParameterExpression readerExp);
        internal abstract Expression GetSerializerExpression(ParameterExpression instanceExp, ParameterExpression writerExp);
        internal abstract void Serialize(Writer writer, object obj);
        internal abstract object Deserialize(Reader reader);
        internal abstract void InitSerializers();
        internal abstract void InitDeserializer();
    }
}
