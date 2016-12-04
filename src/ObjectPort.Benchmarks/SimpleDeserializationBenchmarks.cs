namespace ObjectPort.Benchmarks
{
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Attributes.Exporters;
    using BenchmarkDotNet.Attributes.Jobs;
    using Serializers;

    [ClrJob, CoreJob]
    [MarkdownExporter, AsciiDocExporter, HtmlExporter, CsvExporter, RPlotExporter]
    public class SimpleDeserializationBenchmarks : SerializationBenchmarks
    {
        private TestClass _testObj;

        public SimpleDeserializationBenchmarks()
        {
            Inititalize(new[] { typeof(TestClass), typeof(TestClass2), typeof(TestClass3) });
        }

        [Setup]
        public void Setup()
        {
            _testObj = TestClass.Create();
            foreach (var serializer in _serializers)
            {
                serializer.Value.InitializeIteration();
                serializer.Value.Serialize(_testObj);
            }
        }

        [Cleanup]
        public void Cleanup()
        {
            foreach (var serializer in _serializers)
                serializer.Value.CleanupIteration();
        }

#if !NETCORE
        [Benchmark]
        public void NetSerializer()
        {
            _serializers[typeof(NetSerializaerSerializer)].Deserialize<TestClass>();
        }

        [Benchmark]
        public void MessageShark()
        {
            _serializers[typeof(MessageSharkSerializer)].Deserialize<TestClass>();
        }

        [Benchmark]
        public void SalarBois()
        {
            _serializers[typeof(SalarBoisSerializer)].Deserialize<TestClass>();
        }

        public void Avro()
        {
        }
#endif

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
