namespace ObjectPort.Benchmarks.Serializers
{
    using MsgPack.Serialization;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class MsgPackSerializer : ISerializer
    {
        private static MessagePackSerializer _serializer;

        public void Initialize(IEnumerable<Type> types)
        {
            _serializer = SerializationContext.Default.GetSerializer(types.First());

            Serializer.RegisterTypes(types);
        }

        public void Serialize<T>(Stream stream, T obj)
        {
            _serializer.Pack(stream, obj);
        }

        public T Deserialize<T>(Stream stream)
        {
            return (T)_serializer.Unpack(stream);
        }
    }
}
