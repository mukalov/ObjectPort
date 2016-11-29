namespace ObjectPort.Benchmarks.Serializers
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class ObjectPortSerializer : ISerializer
    {
        public void Initialize(IEnumerable<Type> types)
        {
            Serializer.RegisterTypes(types);
        }

        public void Serialize<T>(Stream stream, T obj)
        {
            Serializer.Serialize(stream, obj);
        }

        public T Deserialize<T>(Stream stream)
        {
            return (T)Serializer.Deserialize(stream);
        }
    }
}
