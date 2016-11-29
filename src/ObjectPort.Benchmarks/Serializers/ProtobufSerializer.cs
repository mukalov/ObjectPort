namespace ObjectPort.Benchmarks.Serializers
{
    using ProtoBuf.Meta;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;

    public class ProtobufSerializer : ISerializer
    {
        private RuntimeTypeModel _protobufModel;

        public void Initialize(IEnumerable<Type> types)
        {
            const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance;
            _protobufModel = RuntimeTypeModel.Default;
            var index = 1;
            foreach (var type in types)
            {
                var metaType = _protobufModel.Add(type, false);
                foreach (var field in type.GetTypeInfo().GetFields(bindingFlags))
                {
                    metaType.Add(index++, field.Name);
                }
                foreach (var prop in type.GetTypeInfo().GetProperties(bindingFlags))
                {
                    metaType.Add(index++, prop.Name);
                }
            }
            _protobufModel.CompileInPlace();
        }

        public void Serialize<T>(Stream stream, T obj)
        {
            ProtoBuf.Serializer.Serialize(stream, obj);
        }

        public T Deserialize<T>(Stream stream)
        {
            return ProtoBuf.Serializer.Deserialize<T>(stream);
        }
    }
}
