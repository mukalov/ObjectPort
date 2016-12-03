namespace ObjectPort.Benchmarks
{
    using System;

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
}
