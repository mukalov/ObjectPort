#if !NETCORE
namespace ObjectPort.Benchmarks.Serializers
{
    using Salar.Bois;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class SalarBoisSerializer : ISerializer
    {
        private readonly BoisSerializer _serializer = new BoisSerializer();

        public void Initialize(IEnumerable<Type> types)
        {
            _serializer.Initialize(types.ToArray());
        }

        public void Serialize<T>(Stream stream, T obj)
        {
            _serializer.Serialize(obj, stream);
        }

        public T Deserialize<T>(Stream stream)
        {
            return _serializer.Deserialize<T>(stream);
        }
    }
}
#endif