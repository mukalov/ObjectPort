#if !NETCORE
namespace ObjectPort.Benchmarks.Serializers
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class NetSerializaerSerializer : ISerializer
    {
        private NetSerializer.Serializer _netSerializer;

        public void Initialize(IEnumerable<Type> types)
        {
            _netSerializer = new NetSerializer.Serializer(types);
        }

        public void Serialize<T>(Stream stream, T obj)
        {
            _netSerializer.Serialize(stream, obj);
        }

        public T Deserialize<T>(Stream stream)
        {
            return (T)_netSerializer.Deserialize(stream);
        }
    }
}
#endif