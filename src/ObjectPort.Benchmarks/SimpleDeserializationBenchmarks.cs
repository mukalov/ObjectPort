namespace ObjectPort.Benchmarks
{
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Attributes.Exporters;
    using BenchmarkDotNet.Attributes.Jobs;
    using Serializers;
    using System;
    using System.Linq;

    [ClrJob, CoreJob]
    [MarkdownExporter, AsciiDocExporter, HtmlExporter, CsvExporter, RPlotExporter]
    public class SimpleDeserializationBenchmarks : SerializationBenchmarks
    {
        private TestClass _testObj;
        private Random _rnd;

        public SimpleDeserializationBenchmarks()
        {
            _rnd = new Random();
            Inititalize(new[] { typeof(TestClass), typeof(TestClass2), typeof(TestClass3) });
        }

        [Setup]
        public void Setup()
        {
            var strGen = new StringGenerator();
            _testObj = new TestClass
            {
                Field1 = strGen.Generate(20, 50),
                Field2 = _rnd.Next(0, int.MaxValue),
                Prop1 = _rnd.Next(0, int.MaxValue),
                Prop2 = new TestClass2
                {
                    Field1 = strGen.Generate(20, 50),
                    Field2 = _rnd.Next(0, int.MaxValue),
                    Prop1 = _rnd.Next(0, int.MaxValue)
                },
                Prop3 = Enumerable.Range(0, 20).Select(i => new TestClass3 { Field1 = strGen.Generate(20, 50), Field2 = i }).ToArray()
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
