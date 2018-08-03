namespace ObjectPort.Benchmarks
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    public interface ISerializerWrapper
    {
        void Initialize(IEnumerable<Type> types);
        void InitializeIteration();
        void CleanupIteration();
        void Serialize<T>(T obj);
        void Serialize<T>(Stream stream, T obj);
        T Deserialize<T>();
        T Deserialize<T>(Stream stream);
    }
}
