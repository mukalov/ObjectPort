namespace ObjectPort.Benchmarks
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class SerializerWrapper<SerT> : ISerializerWrapper
        where SerT : ISerializer, new()
    {
        private SerT _serializer;
        private Stream _stream;

        public SerializerWrapper()
        {
            _serializer = new SerT();
        }

        public void Initialize(IEnumerable<Type> types)
        {
            _serializer.Initialize(types);
        }

        public void InitializeIteration()
        {
            _stream = new MemoryStream();
        }

        public void CleanupIteration()
        {
            _stream?.Dispose();
        }

        public void Serialize<T>(T obj)
        {
            _stream.Seek(0, SeekOrigin.Begin);
            _serializer.Serialize(_stream, obj);
        }

        public void Serialize<T>(Stream stream, T obj)
        {
            stream.Seek(0, SeekOrigin.Begin);
            _serializer.Serialize(stream, obj);
        }

        public T Deserialize<T>()
        {
            _stream.Seek(0, SeekOrigin.Begin);
            return _serializer.Deserialize<T>(_stream);
        }
    }
}
