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
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;

    internal abstract class TypeDescription
    {
        private MemberDescription[] _descriptions;
        private Dictionary<string, int> _orderMapping;
        private Action<object, BinaryWriter> _serializer;
        private Func<BinaryReader, object> _deserializer;
        private readonly Lazy<bool> _isInitialized;

        public Type Type { get; }
        public ushort TypeId { get; private set; }
        public IEnumerable<MemberDescription> Descriptions => _descriptions;

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

        internal abstract Expression GetDeserializerExpression(ParameterExpression readerExpression);
        internal abstract MemberDescription[] GetDescriptions(SerializerState state);

        internal void Serialize(BinaryWriter writer, object obj)
        {
            _serializer.Invoke(obj, writer);
        }

        internal object Deserialize(BinaryReader reader)
        {
            return _deserializer(reader);
        }

        private MemberDescription GetMemeberDescription(string name)
        {
            int index;
            if (!_orderMapping.TryGetValue(name, out index))
                return null;
            Debug.Assert(index < _descriptions.Length);
            return _descriptions[index];
        }

        private void InitDescriptions(SerializerState state)
        {
            _descriptions = GetDescriptions(state);
            _orderMapping = new Dictionary<string, int>();
            for (var i = 0; i < _descriptions.Length; i++)
                _orderMapping[_descriptions[i].Name] = i;

            foreach (var description in _descriptions)
                description.NestedTypeDescription?.InitDescriptions(state);
        }

        private void InitSerializers()
        {
            var instanceExp = Expression.Parameter(typeof(object), "instance");
            var writerExp = Expression.Parameter(typeof(BinaryWriter), "writer");

            foreach (var p in Type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                var pd = GetMemeberDescription(p.Name);
                pd?.CompileSerializer(instanceExp, writerExp);
            }

            foreach (var f in Type.GetFields(BindingFlags.Instance | BindingFlags.Public))
            {
                var fd = GetMemeberDescription(f.Name);
                fd?.CompileSerializer(instanceExp, writerExp);
            }

            foreach (var description in _descriptions)
                description.NestedTypeDescription?.InitSerializers();

            var serializer = Expression.Lambda<Action<object, BinaryWriter>>(
                Expression.Block(_descriptions.Select(d => d.SerializerExpression)),
                instanceExp,
                writerExp);
            _serializer = serializer.Compile();
        }

        private void InitDeserializer()
        {
            var readerExp = Expression.Parameter(typeof(BinaryReader), "reader");
            var deserializerExp = GetDeserializerExpression(readerExp);
            if (Type.IsValueType)
                deserializerExp = Expression.TypeAs(GetDeserializerExpression(readerExp), typeof(object));
            var lamdaExp = Expression.Lambda<Func<BinaryReader, object>>(deserializerExp, readerExp);
            _deserializer = lamdaExp.Compile();
        }
    }
}
