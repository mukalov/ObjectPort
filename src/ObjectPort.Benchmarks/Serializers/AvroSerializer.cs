#if !NETCORE
namespace ObjectPort.Benchmarks.Serializers
{
    using Microsoft.Hadoop.Avro;
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class AvroSerializer<ObjT> : ISerializer
    {
        private readonly IAvroSerializer<ObjT> _serializer;

        public AvroSerializer()
        {
            _serializer = Microsoft.Hadoop.Avro.AvroSerializer.Create<ObjT>();
        }

        public void Initialize(IEnumerable<Type> types)
        {
        }
        
        private void Serialize(Stream stream, object obj)
        {
            _serializer.Serialize(stream, (ObjT)obj);
        }

        private object Deserialize(Stream stream)
        {
            return _serializer.Deserialize(stream);
        }

        public void Serialize<T>(Stream stream, T obj)
        {
            Serialize(stream, (object)obj);
        }

        public T Deserialize<T>(Stream stream)
        {
            return (T)Deserialize(stream);
        }
    }
}
#endif
