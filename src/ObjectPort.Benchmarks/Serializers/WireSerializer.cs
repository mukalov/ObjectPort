namespace ObjectPort.Benchmarks.Serializers
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class WireSerializer : ISerializer
    {
        private readonly Wire.Serializer _serializer = new Wire.Serializer(new Wire.SerializerOptions(false, true));

        public void Initialize(IEnumerable<Type> types)
        {
        }

        public void Serialize<T>(Stream stream, T obj)
        {
            _serializer.Serialize(obj, stream);
        }

        public T Deserialize<T>(Stream stream)
        {
            return (T)_serializer.Deserialize(stream);
        }
    }
}
