namespace ObjectPort.Benchmarks
{
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Attributes.Jobs;
    using ProtoBuf.Meta;
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    public class SimpleSerializationBenchmarks
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
        private MemoryStream _stream;
        private RuntimeTypeModel _protobufModel;
        private Random _rnd;
#if !NETCORE
        private NetSerializer.Serializer _netSerializer;
#endif
        public SimpleSerializationBenchmarks()
        {
            _rnd = new Random();
            SetupProtobuf();

            _stream = new MemoryStream();
            Serializer.RegisterTypes(new[] { typeof(TestClass) });
#if !NETCORE
            _netSerializer = new NetSerializer.Serializer(new[] { typeof(TestClass) });
#endif
        }

        private void SetupProtobuf()
        {
            _protobufModel = RuntimeTypeModel.Default;
            _protobufModel.Add(typeof(TestClass), false)
               .Add(1, "Field1")
               .Add(2, "Field2")
               .Add(4, "Prop1");

            _protobufModel.CompileInPlace();
        }

        [Setup]
        public void Setup()
        {

        }

        private void SerializeDeserialize(Action<Stream, TestClass> serializeHandler, Func<Stream, TestClass> deserializeHanlder)
        {
            var testObj = new TestClass
            {
                Field1 = "SDFsdfsdfdsfsdf sdfsdf",
                Field2 = _rnd.Next(0, int.MaxValue),
                Prop1 = _rnd.Next(0, int.MaxValue)
            };
            using (var stream = new MemoryStream())
            {
                serializeHandler(stream, testObj);
                stream.Seek(0, SeekOrigin.Begin);
                var obj = deserializeHanlder(stream);
            }
        }

        [Benchmark]
        public void ProtobufSerialize()
        {
            SerializeDeserialize(
                (s, o) => ProtoBuf.Serializer.Serialize(s, o), 
                s => ProtoBuf.Serializer.Deserialize<TestClass>(s));
        }

#if !NETCORE
        [Benchmark]
        public void NetSerializerSerialize()
        {
            SerializeDeserialize(
                (s, o) => _netSerializer.Serialize(s, o),
                s => (TestClass)_netSerializer.Deserialize(s));
        }
#endif

        [Benchmark]
        public void ObjectPortSerialize()
        {
            SerializeDeserialize(
                (s, o) => Serializer.Serialize(s, o),
                s => (TestClass)Serializer.Deserialize(s));
        }
    }
}
