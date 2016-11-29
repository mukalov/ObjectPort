namespace ObjectPort.Benchmarks
{
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Attributes.Exporters;
    using BenchmarkDotNet.Attributes.Jobs;
    using ProtoBuf.Meta;
    using Serializers;
    using System;
    using System.IO;


    [ClrJob, CoreJob]
    [MarkdownExporter, AsciiDocExporter, HtmlExporter, CsvExporter, RPlotExporter]
    public class SimpleSerializationBenchmarks : SerializationBenchmark
    {
#if !NETCORE
        [Serializable]
#endif
        public class TestClass
        {
            public string Field1;
            public int Field2;
            public int Prop1;
        }

        private TestClass _testObj;
        private Random _rnd;

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
        }

        [Benchmark]
        public void ProtobufSerialize()
        {
            var serializer = _serializers[typeof(ProtobufSerializer)];
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, _testObj);
            }
        }

#if !NETCORE
        [Benchmark]
        public void NetSerializerSerialize()
        {
            var serializer = _serializers[typeof(NetSerializaerSerializer)];
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, _testObj);
            }
        }
#endif
        [Benchmark]
        public void WireSerialize()
        {
            var serializer = _serializers[typeof(WireSerializer)];
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, _testObj);
            }
        }


        [Benchmark]
        public void ObjectPortSerialize()
        {
            var serializer = _serializers[typeof(ObjectPortSerializer)];
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, _testObj);
            }
        }
    }
}
