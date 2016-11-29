namespace ObjectPort.Benchmarks
{
    using Serializers;
    using System;
    using System.Collections.Generic;

    public class SerializationBenchmark
    {
        protected IDictionary<Type, ISerializer> _serializers;

        public ISerializer Serializer { get; set; }

        public SerializationBenchmark()
        {
            _serializers = new Dictionary<Type, ISerializer>
            {
#if !NETCORE
                [typeof(NetSerializaerSerializer)] = new NetSerializaerSerializer(),
#endif
                [typeof(ObjectPortSerializer)] = new ObjectPortSerializer(),
                [typeof(ProtobufSerializer)] = new ProtobufSerializer(),
                [typeof(WireSerializer)] = new WireSerializer(),
            };
        }

        protected void Inititalize(IEnumerable<Type> types)
        {
            foreach (var serializer in _serializers)
                serializer.Value.Initialize(types);
        }
    }
}
