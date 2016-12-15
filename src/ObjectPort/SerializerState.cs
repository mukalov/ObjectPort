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

namespace ObjectPort
{
    using Common;
    using Descriptions;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;

    internal class SerializerState
    {
        internal const ushort CustomTypeIdsStart = 64;

        private AdaptiveHashtable<TypeDescription> _descriptions;
        internal Dictionary<Type, TypeDescription> AllTypeDescriptions;
        internal ushort LastTypeId = CustomTypeIdsStart;
        internal TypeDescription[] DescriptionsById;

        internal SerializerState()
        {
            AllTypeDescriptions = new Dictionary<Type, TypeDescription>();
            _descriptions = new AdaptiveHashtable<TypeDescription>();
        }

        internal SerializerState Clone()
        {
            var copy = new SerializerState
            {
                AllTypeDescriptions = AllTypeDescriptions.ToDictionary(i => i.Key, i => i.Value),
                LastTypeId = LastTypeId,
                _descriptions = _descriptions.Clone()
            };
            return copy;
        }

        internal void AddDescription(Type type, TypeDescription description)
        {
            Debug.Assert(_descriptions != null, "Descriptions can't be null");
            _descriptions.AddValue((uint)type.TypeHandle.GetHashCode(), description);
            if (!AllTypeDescriptions.ContainsKey(type))
                AllTypeDescriptions.Add(type, description);
        }

        internal void AddDescription(ushort code, TypeDescription description)
        {
            Debug.Assert(_descriptions != null, "Descriptions can't be null");
            _descriptions.AddValue(code, description);
            if (!AllTypeDescriptions.ContainsKey(description.Type))
                AllTypeDescriptions.Add(description.Type, description);
        }

        internal TypeDescription GetDescription(Type type)
        {
            Debug.Assert(_descriptions != null, "Descriptions can't be null");
            return _descriptions.TryGetValue((uint)type.TypeHandle.GetHashCode());
        }

        internal TypeDescription GetDescription(int code)
        {
            Debug.Assert(_descriptions != null, "Descriptions can't be null");
            return _descriptions.TryGetValue((uint)code);
        }

        internal IEnumerable<TypeDescription> GetDescriptionsForDerivedTypes(Type type)
        {
            foreach (var description in AllTypeDescriptions.Select(i => i.Value).ToArray())
            {
                if (description.Type == type
                    || description.Type.GetTypeInfo().IsAssignableFrom(type)
                    || description.Type.GetTypeInfo().IsSubclassOf(type)
                    || type.GetTypeInfo().IsInterface && description.Type.GetTypeInfo().GetInterfaces().Contains(type))
                {
                    yield return description;
                }
            }
        }
    }
}
