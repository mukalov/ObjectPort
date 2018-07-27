namespace ObjectPort.Benchmarks
{
    using BenchmarkDotNet.Attributes;
    using ObjectPort.Benchmarks.Serializers;

    [CoreJob]
    public class SimpleDeserializationBenchmarksCore : SerializationBenchmarks
    {
        private TestClass _testObj;

        public SimpleDeserializationBenchmarksCore()
        {
            Inititalize(new[] { typeof(TestClass), typeof(TestClass2), typeof(TestClass3) });
        }

        [GlobalSetup]
        public void Setup()
        {
            _testObj = TestClass.Create();
            foreach (var serializer in _serializers)
            {
                serializer.Value.InitializeIteration();
                serializer.Value.Serialize(_testObj);
            }
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
            _serializers[typeof(ProtobufSerializer)].Deserialize<TestClass>();
        }

        [Benchmark]
        public void Wire()
        {
            _serializers[typeof(WireSerializer)].Deserialize<TestClass>();
        }

        [Benchmark]
        public void ObjectPort()
        {
            _serializers[typeof(ObjectPortSerializer)].Deserialize<TestClass>();
        }

    }
}
