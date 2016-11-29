namespace ObjectPort.Benchmarks
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    public interface ISerializer
    {
        void Initialize(IEnumerable<Type> types);
        void Serialize<T>(Stream stream, T obj);
        T Deserialize<T>(Stream stream);
    }
}
