#if !NETCORE
namespace ObjectPort.Benchmarks.Serializers
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class MessageSharkSerializer : ISerializer
    {
        public void Initialize(IEnumerable<Type> types)
        {
        }

        public void Serialize<T>(Stream stream, T obj)
        {
            MessageShark.MessageSharkSerializer.Serialize(obj, stream);
        }

        public T Deserialize<T>(Stream stream)
        {
            return MessageShark.MessageSharkSerializer.Deserialize<T>(stream);
        }

    }
}
#endif