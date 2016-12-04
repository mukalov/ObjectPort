namespace ObjectPort.Benchmarks
{
    using System;
    using System.Linq;

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

        public static TestClass Create()
        {
            var strGen = new StringGenerator();
            var rnd = new Random();
            return new TestClass
            {
                Field1 = strGen.Generate(20, 50),
                Field2 = rnd.Next(0, int.MaxValue),
                Prop1 = rnd.Next(0, int.MaxValue),
                Prop2 = new TestClass2
                {
                    Field1 = strGen.Generate(20, 50),
                    Field2 = rnd.Next(0, int.MaxValue),
                    Prop1 = rnd.Next(0, int.MaxValue)
                },
                Prop3 = Enumerable.Range(0, 20).Select(i => new TestClass3 { Field1 = strGen.Generate(20, 50), Field2 = i }).ToArray()
            };
        }
    }
}
