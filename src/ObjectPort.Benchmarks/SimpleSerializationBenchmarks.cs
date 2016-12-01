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
    public class SimpleSerializationBenchmarks : SerializationBenchmarks
    {
#if !NETCORE
        [Serializable]
#endif
        public class TestClass2
        {
            public string Field1 { get; set; }
            public int Field2 { get; set; }
            public int Prop1 { get; set; }
        }

#if !NETCORE
        [Serializable]
#endif
        public class TestClass3
        {
            public string Field1 { get; set; }
            public int Field2 { get; set; }
        }

#if !NETCORE
        [Serializable]
#endif
        public class TestClass
        {
            public string Field1 { get; set; }
            public int Field2 { get; set; }
            public int Prop1 { get; set; }
            public TestClass2 Prop2 { get; set; }

            public TestClass3[] Prop3 { get; set; }
        }

        private TestClass _testObj;
        private Random _rnd;

        public SimpleSerializationBenchmarks()
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
        public void NetSerializerSerialize()
        {
            _serializers[typeof(NetSerializaerSerializer)].Serialize(_testObj);
        }

        [Benchmark]
        public void MessageSharkSerialize()
        {
            _serializers[typeof(MessageSharkSerializer)].Serialize(_testObj);
        }

        [Benchmark]
        public void SalarBoisSerialize()
        {
            _serializers[typeof(SalarBoisSerializer)].Serialize(_testObj);
        }

        public void AvroSerialize()
        {
        }
#endif

        [Benchmark]
        public void ProtobufSerialize()
        {
            _serializers[typeof(ProtobufSerializer)].Serialize(_testObj);
        }

        [Benchmark]
        public void WireSerialize()
        {
            _serializers[typeof(WireSerializer)].Serialize(_testObj);
        }


        [Benchmark]
        public void ObjectPortSerialize()
        {
            _serializers[typeof(ObjectPortSerializer)].Serialize(_testObj);
        }
    }
}
