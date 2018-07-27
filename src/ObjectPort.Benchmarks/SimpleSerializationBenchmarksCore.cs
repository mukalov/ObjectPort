namespace ObjectPort.Benchmarks
{
    using BenchmarkDotNet.Attributes;
    using ObjectPort.Benchmarks.Serializers;

    [CoreJob]
    public class SimpleSerializationBenchmarksCore : SerializationBenchmarks
    {
        private TestClass _testObj;

        public SimpleSerializationBenchmarksCore()
        {
            Inititalize(new[] { typeof(TestClass), typeof(TestClass2), typeof(TestClass3) });
        }

        [GlobalSetup]
        public void Setup()
        {
            _testObj = TestClass.Create();
            foreach (var serializer in _serializers)
                serializer.Value.InitializeIteration();
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            foreach (var serializer in _serializers)
                serializer.Value.CleanupIteration();
        }

        [Benchmark]
        public void Protobuf()
        {
            _serializers[typeof(ProtobufSerializer)].Serialize(_testObj);
        }

        [Benchmark]
        public void Wire()
        {
            _serializers[typeof(WireSerializer)].Serialize(_testObj);
        }

        [Benchmark]
        public void ObjectPort()
        {
            _serializers[typeof(ObjectPortSerializer)].Serialize(_testObj);
        }
    }
}
