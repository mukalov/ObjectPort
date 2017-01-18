#region License
//Copyright(c) 2016 Dmytro Mukalov

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.
#endregion

namespace ObjectPort.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Xunit;

#if !NET40
    [Collection("ObjectPort")]
#endif
    public class CommonTests : TestsBase
    {
        public class TestClass1
        {
            public string Prop1 { get; set; }
            public int Prop2 { get; set; }

            public override bool Equals(object obj)
            {
                var o = obj as TestClass1;
                if (o == null)
                    return false;
                return o.Prop1 == Prop1 && o.Prop2 == Prop2;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }

        public class TesClass2
        {
            public TestClass1 Prop1 { get; set; }
            public string Prop2 { get; set; }
            public int Prop3 { get; set; }

            public override bool Equals(object obj)
            {
                var o = obj as TesClass2;
                if (o == null)
                    return false;
                return o.Prop1.Equals(Prop1) && o.Prop2 == Prop2 && o.Prop3 == Prop3;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }

        public class TestClass3
        {
            public TesClass2 Prop1 { get; set; }
            public string Prop2 { get; set; }
            public int Prop3 { get; set; }

            public override bool Equals(object obj)
            {
                var o = obj as TestClass3;
                if (o == null)
                    return false;
                return o.Prop1.Equals(Prop1) && o.Prop2 == Prop2 && o.Prop3 == Prop3;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }

        [Fact]
        public void Should_Serialize_Anonymous_Type()
        {
            var testObj = new
            {
                Field1 = 343,
                Field2 = "45454",
                Field3 = new
                {
                    Field7 = new TestCustomClass
                    {
                        IntField = 555,
                        StrField = "444444"
                    },
                    Field8 = "Test Test"
                }
            };

            Serializer.RegisterTypes(new[] { testObj.GetType() });
            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, testObj);
                stream.Seek(0, SeekOrigin.Begin);
                var result = Serializer.Deserialize(stream);
                Assert.IsType(testObj.GetType(), result);
                Assert.Equal(result, testObj);
            }
        }

        [Fact]
        public void Should_Serialize_Complex_Object()
        {
            var testObj = new TestClass3
            {
                Prop1 = new TesClass2
                {
                    Prop1 = new TestClass1
                    {
                        Prop1 = "Test 1",
                        Prop2 = 23423
                    },
                    Prop2 = "Test 2",
                    Prop3 = 423432
                },
                Prop2 = "Test 3",
                Prop3 = 657567565
            };
            Serializer.RegisterTypes(new[] { typeof(TestClass3) });
            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, testObj);
                stream.Seek(0, SeekOrigin.Begin);
                var result = Serializer.Deserialize(stream);
                Assert.IsType(testObj.GetType(), result);
                Assert.Equal(result, testObj);
            }
        }

        [Fact]
        public void Should_Serialize_Type_By_Id()
        {
            Serializer.RegisterTypes(new Dictionary<ushort, Type>() { [64] = typeof(TestCustomClass) });
            var testObj = new TestCustomClass { IntField = 45345, StrField = "Test 1" };
            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, testObj);
                stream.Seek(0, SeekOrigin.Begin);
                var result = Serializer.Deserialize(stream);
                Assert.IsType(testObj.GetType(), result);
                Assert.Equal(result, testObj);
            }
        }

        [Fact]
        public void Shouldnt_Serialize_Unknown_Type()
        {
            var testObj = new TestCustomClass { IntField = 45345, StrField = "Test 1" };
            Action serializer = () =>
            {
                using (var stream = new MemoryStream())
                {
                    Serializer.Serialize(stream, testObj);
                }
            };

#if !NET40
            var message = "Serialization is not supported for the type";
#endif
            var ex = Assert.Throws<NotSupportedException>(serializer);
#if !NET40
            Assert.StartsWith(message, ex.Message);
#endif
        }

        [Fact]
        public void Shouldnt_Desserialize_Unknown_Type()
        {
            Serializer.RegisterTypes(new Dictionary<ushort, Type>() { [100] = typeof(TestCustomClass) });
            var testObj = new TestCustomClass { IntField = 45345, StrField = "Test 1" };
            Action serializer = () =>
            {
                using (var stream = new MemoryStream())
                {
                    Serializer.Serialize(stream, testObj);
                    Serializer.Clear();
                    stream.Seek(0, SeekOrigin.Begin);
                    Serializer.Deserialize(stream);
                }
            };

#if !NET40
            var message = "Unknown type id";
#endif
            var ex = Assert.Throws<ArgumentOutOfRangeException>(serializer);
#if !NET40
            Assert.Contains(message, ex.Message);
#endif
        }


        [Fact]
        public void Should_Serialize_Struct_Polymorphic_Hierarchy_From_Interface()
        {
            Serializer.RegisterTypes(new[] { typeof(TestCustomStructDerived1) });
            var obj = new TestCustomStructDerived1 { IntProp = 3453, StrProp = "Test 1" };
            TestStructField<IBaseInterface>(obj);
            TestStructProperty<IBaseInterface>(obj);
            TestClassField<IBaseInterface>(obj);
            TestClassProperty<IBaseInterface>(obj);
        }

        [Fact]
        public void Should_Serialize_Class_Polymorphic_Hierarchy_From_Interface()
        {
            Serializer.RegisterTypes(new[] { typeof(TestCustomClassDerived3) });
            var obj = new TestCustomClassDerived3 { StrField = "Test 1", StrProp1 = "Test 2" };
            TestStructField<IBaseInterface>(obj);
            TestStructProperty<IBaseInterface>(obj);
            TestClassField<IBaseInterface>(obj);
            TestClassProperty<IBaseInterface>(obj);
        }

        [Fact]
        public void Should_Serialize_Class_Polymorphic_Hierarchy_From_Abstract_Class()
        {
            Serializer.RegisterTypes(new[] { typeof(TestCustomClassDerived3) });
            var obj = new TestCustomClassDerived3 { StrField = "Test 1", StrProp1 = "Test 2" };
            TestStructField<TestCustomClassAbstract>(obj);
            TestStructProperty<TestCustomClassAbstract>(obj);
            TestClassField<TestCustomClassAbstract>(obj);
            TestClassProperty<TestCustomClassAbstract>(obj);
        }

        [Fact]
        public void Shouldnt_Serialize_Null_Object()
        {
            Action serializerByType = () =>
            {
                using (var stream = new MemoryStream())
                {
                    Serializer.Serialize<TestCustomClass>(stream, null);
                }
            };

            Action serializerByObject = () =>
            {
                using (var stream = new MemoryStream())
                {
                    Serializer.Serialize(stream, null);
                }
            };


#if !NET40
            var message = "Cannot serialize null object";
#endif
            var ex = Assert.Throws<ArgumentException>(serializerByType);
#if !NET40
            Assert.Contains(message, ex.Message);
#endif
            ex = Assert.Throws<ArgumentException>(serializerByObject);
#if !NET40
            Assert.Contains(message, ex.Message);
#endif
        }

        [Fact]
        public void Shouldnt_Register_Invalid_Id()
        {
#if !NET40
            var message = "Invalid type id";
#endif
            Serializer.RegisterTypes(new Dictionary<ushort, Type>() { [100] = typeof(TestCustomClass) });

            var ex = Assert.Throws<ArgumentException>(() => { Serializer.RegisterTypes(new Dictionary<ushort, Type>() { [65001] = typeof(TestCustomClass) }); });
#if !NET40
            Assert.Contains(message, ex.Message);
#endif
        }
    }
}
