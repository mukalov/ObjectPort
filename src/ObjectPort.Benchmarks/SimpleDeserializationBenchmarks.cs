namespace ObjectPort.Benchmarks
{
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Attributes.Exporters;
    using BenchmarkDotNet.Attributes.Jobs;
    using Serializers;
    using System;

    [ClrJob, CoreJob]
    [MarkdownExporter, AsciiDocExporter, HtmlExporter, CsvExporter, RPlotExporter]
    public class SimpleDeserializationBenchmarks : SerializationBenchmarks
    {
#if !NETCORE
        [Serializable]
#endif
        public class TestClass
        {
            public string Field1 { get; set; }
            public int Field2 { get; set; }
            public int Prop1 { get; set; }
        }

        private TestClass _testObj;
        private Random _rnd;

        public SimpleDeserializationBenchmarks()
        {
            _rnd = new Random();
            Inititalize(new[] { typeof(TestClass) });
        }

        [Setup]
        public void Setup()
        {
            _testObj = new TestClass
            {
                Field1 = new StringGenerator().Generate(20, 50),
                Field2 = _rnd.Next(0, int.MaxValue),
                Prop1 = _rnd.Next(0, int.MaxValue)
            };
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
        public void NetSerializerDeserialize()
        {
            _serializers[typeof(NetSerializaerSerializer)].Deserialize<TestClass>();
        }

        [Benchmark]
        public void MessageSharkDeserialize()
        {
            _serializers[typeof(MessageSharkSerializer)].Deserialize<TestClass>();
        }

        [Benchmark]
        public void SalarBoisDeserialize()
        {
            _serializers[typeof(SalarBoisSerializer)].Deserialize<TestClass>();
        }

        public void AvroDeserialize()
        {
        }
#endif

        [Benchmark]
        public void ProtobufDeserialize()
        {
            _serializers[typeof(ProtobufSerializer)].Deserialize<TestClass>();
        }

        [Benchmark]
        public void WireDeserialize()
        {
            _serializers[typeof(WireSerializer)].Deserialize<TestClass>();
        }

        [Benchmark]
        public void ObjectPortDeserialize()
        {
            _serializers[typeof(ObjectPortSerializer)].Deserialize<TestClass>();
        }
    }
}
