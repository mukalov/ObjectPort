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
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    internal class ComplexTypeDescription<T> : SpecializedTypeDescription<T>
    {
        private MemberDescription[] _descriptions;
        private Dictionary<string, int> _orderMapping;

        public IEnumerable<MemberDescription> Descriptions => _descriptions.OrderBy(d => d.Name);

        public ComplexTypeDescription(ushort typeId, Type type, SerializerState state) 
            : base(typeId, type, state)
        {
        }

        internal override Expression GetSerializerExpression(ParameterExpression instanceExp, ParameterExpression writerExp)
        {
            foreach (var p in Type.GetTypeInfo().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                var pd = GetMemeberDescription(p.Name);
                pd?.CompileSerializer(instanceExp, writerExp);
            }

            foreach (var f in Type.GetTypeInfo().GetFields(BindingFlags.Instance | BindingFlags.Public))
            {
                var fd = GetMemeberDescription(f.Name);
                fd?.CompileSerializer(instanceExp, writerExp);
            }

            foreach (var description in _descriptions)
                description.NestedTypeDescription?.InitSerializers();

            return Expression.Block(Descriptions.Select(d => d.SerializerExpression));
        }

        internal override Expression GetDeserializerExpression(ParameterExpression readerExpression)
        {
            var memberAssignments = new List<MemberAssignment>();
            foreach (var description in Descriptions)
            {
                var valueExp = description.DeserializeExpression(readerExpression);
                memberAssignments.Add(description.GetAssignment(valueExp));
            }
            var initExp = Expression.MemberInit(Expression.New(Type), memberAssignments);
            return initExp;
        }

        internal override MemberDescription[] GetDescriptions(SerializerState state)
        {
            const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance;
            var fileds = Type.GetTypeInfo().GetFields(bindingFlags)
                .Select(i => new FieldDescription(i, state)
                {
                    NestedTypeDescription = Serializer.GetTypeDescription(i.FieldType, state)
                });
            var properties = Type.GetTypeInfo().GetProperties(bindingFlags)
                .Select(p => new PropertyDescription(p, state)
                {
                    NestedTypeDescription = Serializer.GetTypeDescription(p.PropertyType, state)
                });
            return fileds.Concat(properties.Cast<MemberDescription>()).ToArray();
        }

        internal override void InitDescriptions(SerializerState state)
        {
            _descriptions = GetDescriptions(state);
            _orderMapping = new Dictionary<string, int>();
            for (var i = 0; i < _descriptions.Length; i++)
                _orderMapping[_descriptions[i].Name] = i;

            foreach (var description in _descriptions)
                description.NestedTypeDescription?.InitDescriptions(state);
        }

        private MemberDescription GetMemeberDescription(string name)
        {
            int index;
            if (!_orderMapping.TryGetValue(name, out index))
                return null;
            Debug.Assert(index < _descriptions.Length, "Index can't be out of range");
            return _descriptions[index];
        }
    }
}
