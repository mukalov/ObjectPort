namespace ObjectPort.Benchmarks
{
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Attributes.Exporters;
    using BenchmarkDotNet.Attributes.Jobs;
    using Serializers;

    [ClrJob, CoreJob]
    [MarkdownExporter, AsciiDocExporter, HtmlExporter, CsvExporter, RPlotExporter]
    public class SimpleSerializationBenchmarks : SerializationBenchmarks
    {
        private TestClass _testObj;

        public SimpleSerializationBenchmarks()
        {
            Inititalize(new[] { typeof(TestClass), typeof(TestClass2), typeof(TestClass3) });
        }

        [Setup]
        public void Setup()
        {
            _testObj = TestClass.Create();
            foreach (var serializer in _serializers)
                serializer.Value.InitializeIteration();
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
            _serializers[typeof(NetSerializaerSerializer)].Serialize(_testObj);
        }

        [Benchmark]
        public void MessageShark()
        {
            _serializers[typeof(MessageSharkSerializer)].Serialize(_testObj);
        }

        [Benchmark]
        public void Salar()
        {
            _serializers[typeof(SalarBoisSerializer)].Serialize(_testObj);
        }

        public void Avro()
        {
        }
#endif

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
