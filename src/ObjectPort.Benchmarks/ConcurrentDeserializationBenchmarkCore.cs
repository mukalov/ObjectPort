namespace ObjectPort.Benchmarks
{
    using BenchmarkDotNet.Attributes;
    using ObjectPort.Benchmarks.Serializers;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    [CoreJob]
    [IterationTime(5000)]
    [AllStatisticsColumn]
    [MarkdownExporter, AsciiDocExporter, HtmlExporter, CsvExporter, RPlotExporter]
    public class ConcurrentDeserializationBenchmarkCore : SerializationBenchmarks
    {
        private const int NumberOfParallelsTasks = 1000;

        private TestClass _testObj;
        private Dictionary<Type, Stream[]> _streams;

        public ConcurrentDeserializationBenchmarkCore()
        {
            _streams = new Dictionary<Type, Stream[]>();
            Inititalize(new[] { typeof(TestClass), typeof(TestClass2), typeof(TestClass3) });
        }

        [GlobalSetup]
        public void Setup()
        {
            _testObj = TestClass.Create();
            foreach (var serializer in _serializers)
            {
                serializer.Value.InitializeIteration();
                var serializerStreams = Enumerable
                    .Range(1, NumberOfParallelsTasks)
                    .Select(i => new MemoryStream())
                    .ToArray();

                foreach (var serializerStream in serializerStreams)
                    serializer.Value.Serialize(serializerStream, _testObj);

                _streams.Add(serializer.Key, serializerStreams);
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
            Deserialize<ProtobufSerializer>();
        }

        [Benchmark]
        public void Wire()
        {
            Deserialize<WireSerializer>();
        }

        [Benchmark]
        public void ObjectPort()
        {
            Deserialize<ObjectPortSerializer>();
        }

        private void Deserialize<T>()
        {
            var serializer = _serializers[typeof(T)];
            var streams = _streams[typeof(T)];
            Parallel.For(0, NumberOfParallelsTasks, i => serializer.Deserialize<TestClass>(streams[i]));
        }

    }
}
