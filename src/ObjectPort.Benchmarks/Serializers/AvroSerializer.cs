namespace ObjectPort.Benchmarks.Serializers
{
    using System;
    using System.Collections.Generic;
    using System.IO;


    public class AvroSerializer : ISerializer
    {
        public void Initialize(IEnumerable<Type> types)
        {
            throw new NotImplementedException();
        }

        public void Serialize<T>(Stream stream, T obj)
        {
            throw new NotImplementedException();
        }

        public T Deserialize<T>(Stream stream)
        {
            throw new NotImplementedException();
        }
    }
}
