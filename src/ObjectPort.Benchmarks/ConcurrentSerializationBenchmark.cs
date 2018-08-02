namespace ObjectPort.Benchmarks
{
    using BenchmarkDotNet.Attributes;
    using ObjectPort.Benchmarks.Serializers;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    [ClrJob]
    [IterationTime(1000)]
    [MarkdownExporter, AsciiDocExporter, HtmlExporter, CsvExporter, RPlotExporter]
    public class ConcurrentSerializationBenchmark : SerializationBenchmarks
    {
        private const int NumberOfParallelsTasks = 1000;

        private TestClass _testObj;
        private Stream[] _streams;

        public ConcurrentSerializationBenchmark()
        {
            Inititalize(new[] { typeof(TestClass), typeof(TestClass2), typeof(TestClass3) });
        }

        [GlobalSetup]
        public void Setup()
        {
            _testObj = TestClass.Create();
            foreach (var serializer in _serializers)
                serializer.Value.InitializeIteration();

            _streams = Enumerable
                .Range(1, NumberOfParallelsTasks)
                .Select(i => new MemoryStream())
                .ToArray();
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            foreach (var serializer in _serializers)
                serializer.Value.CleanupIteration();
        }

#if !NETCORE
        [Benchmark]
        public void NetSerializer()
        {
            Serialize<NetSerializaerSerializer>();
        }

        [Benchmark]
        public void MessageShark()
        {
            Serialize<MessageSharkSerializer>();
        }

        [Benchmark]
        public void Salar()
        {
            Serialize<SalarBoisSerializer>();
        }

        public void Avro()
        {
        }
#endif

        [Benchmark]
        public void Protobuf()
        {
            Serialize<ProtobufSerializer>();
        }

        [Benchmark]
        public void Wire()
        {
            _serializers[typeof(WireSerializer)].Serialize(_testObj);
            Serialize<WireSerializer>();
        }

        [Benchmark]
        public void ObjectPort()
        {
            Serialize<ObjectPortSerializer>();
        }

        private void Serialize<T>()
        {
            var serializer = _serializers[typeof(T)];
            Parallel.For(0, NumberOfParallelsTasks, i => serializer.Serialize(_streams[i], _testObj));
        }
    }
}
