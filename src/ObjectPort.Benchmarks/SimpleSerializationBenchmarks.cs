namespace ObjectPort.Benchmarks
{
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Attributes.Exporters;
    using BenchmarkDotNet.Attributes.Jobs;
    using ProtoBuf.Meta;
    using Serializers;
    using System;
    using System.IO;
    using System.Runtime.Serialization;

    [ClrJob, CoreJob]
    [MarkdownExporter, AsciiDocExporter, HtmlExporter, CsvExporter, RPlotExporter]
    public class SimpleSerializationBenchmarks : SerializationBenchmark
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
        private Stream _stream;

        public SimpleSerializationBenchmarks()
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
            _stream = new MemoryStream();
        }

        [Cleanup]
        public void Cleanup()
        {
            _stream?.Dispose();
        }


        private void Serialize<T>(Type type, T obj)
        {
            _stream.Seek(0, SeekOrigin.Begin);
            var serializer = _serializers[type];
            serializer.Serialize(_stream, obj);
        }

#if !NETCORE
        [Benchmark]
        public void NetSerializerSerialize()
        {
            Serialize(typeof(NetSerializaerSerializer), _testObj);
        }

        [Benchmark]
        public void MessageSharkSerialize()
        {
            Serialize(typeof(MessageSharkSerializer), _testObj);
        }

        [Benchmark]
        public void SalarBoisSerialize()
        {
            Serialize(typeof(SalarBoisSerializer), _testObj);
        }

        [Benchmark]
        public void AvroSerialize()
        {
            _stream.Seek(0, SeekOrigin.Begin);
            var serializer = new AvroSerializer<TestClass>();
            serializer.Serialize(_stream, _testObj);
        }
#endif
        [Benchmark]
        public void ProtobufSerialize()
        {
            Serialize(typeof(ProtobufSerializer), _testObj);
        }

        [Benchmark]
        public void WireSerialize()
        {
            Serialize(typeof(WireSerializer), _testObj);
        }


        [Benchmark]
        public void ObjectPortSerialize()
        {
            Serialize(typeof(ObjectPortSerializer), _testObj);
        }
    }
}
