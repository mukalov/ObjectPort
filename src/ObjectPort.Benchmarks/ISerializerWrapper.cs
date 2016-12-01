namespace ObjectPort.Benchmarks
{
    using System;
    using System.Collections.Generic;

    public interface ISerializerWrapper
    {
        void Initialize(IEnumerable<Type> types);
        void InitializeIteration();
        void CleanupIteration();
        void Serialize<T>(T obj);
        T Deserialize<T>();
    }
}
