namespace ObjectPort.Benchmarks
{
    using BenchmarkDotNet.Attributes;
    using Serializers;
    using System;
    using System.Collections.Generic;

    public class SerializationBenchmarks
    {
        protected IDictionary<Type, ISerializerWrapper> _serializers;

        public ISerializer Serializer { get; set; }

        public SerializationBenchmarks()
        {
            _serializers = new Dictionary<Type, ISerializerWrapper>
            {
#if !NETCORE
                [typeof(NetSerializaerSerializer)] = new SerializerWrapper<NetSerializaerSerializer>(),
                [typeof(MessageSharkSerializer)] = new SerializerWrapper<MessageSharkSerializer>(),
                [typeof(SalarBoisSerializer)] = new SerializerWrapper<SalarBoisSerializer>(),
#endif
                [typeof(ObjectPortSerializer)] = new SerializerWrapper<ObjectPortSerializer>(),
                [typeof(ProtobufSerializer)] = new SerializerWrapper<ProtobufSerializer>(),
                [typeof(WireSerializer)] = new SerializerWrapper<WireSerializer>(),
                [typeof(MsgPackSerializer)] = new SerializerWrapper<MsgPackSerializer>()
            };
        }

        protected void Inititalize(IEnumerable<Type> types)
        {
            foreach (var serializer in _serializers)
                serializer.Value.Initialize(types);
        }
    }
}
