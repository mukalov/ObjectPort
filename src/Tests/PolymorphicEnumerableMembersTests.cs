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
    using System.Linq;
    using Xunit;

    [Collection("ObjectPort")]
    public class PolymorphicEnumerableMembersTests : TestsBase
    {
        private interface IBaseInterface
        {
            string StrProp { get; set; }
            int IntProp { get; set; }
        }

        private struct TestCustomStructDerived1 : IBaseInterface
        {
            public int IntProp { get; set; }
            public string StrProp { get; set; }
        }

        private struct TestCustomStructDerived2 : IBaseInterface
        {
            public int IntProp { get; set; }
            public string StrProp { get; set; }
            public int IntField;
            public string StrField;
        }

        private abstract class TestCustomClassAbstract : IBaseInterface
        {
            public int IntProp { get; set; }
            public string StrProp { get; set; }
            public abstract string StrProp1 { get; set; }
        }

        private class TestCustomClassDerived1 : TestCustomClassAbstract
        {
            public override string StrProp1 { get; set; }

            public override bool Equals(object obj)
            {
                var testObj = obj as TestCustomClassDerived1;
                if (obj == null)
                    return false;
                return StrProp1 == testObj.StrProp1 && StrProp == testObj.StrProp && IntProp == testObj.IntProp;
            }

            public override int GetHashCode()
            {
                return (StrProp + StrProp1 + IntProp.ToString()).GetHashCode();
            }
        }

        private class TestCustomClassDerived2 : IBaseInterface
        {
            public int IntProp { get; set; }
            public string StrProp { get; set; }
            public int IntField;
            public string StrField;

            public override bool Equals(object obj)
            {
                var testObj = obj as TestCustomClassDerived2;
                if (obj == null)
                    return false;
                return StrProp == testObj.StrProp && StrField == testObj.StrField && IntProp == testObj.IntProp && IntField == testObj.IntField;
            }

            public override int GetHashCode()
            {
                return (StrField + StrProp + IntProp.ToString() + IntField.ToString()).GetHashCode();
            }
        }

        private class TestCustomClassDerived3 : TestCustomClassDerived1
        {
            public string StrField;

            public override bool Equals(object obj)
            {
                var testObj = obj as TestCustomClassDerived3;
                if (obj == null)
                    return false;
                return StrProp1 == testObj.StrProp1 && StrProp == testObj.StrProp && IntProp == testObj.IntProp && StrField == testObj.StrField;
            }

            public override int GetHashCode()
            {
                return (StrProp + StrProp1 + StrField + IntProp.ToString()).GetHashCode();
            }
        }

        private static readonly IBaseInterface[] TestPolymorphicArrayOfStructs1 = new IBaseInterface[]
        {
            new TestCustomStructDerived1 { IntProp = 324, StrProp ="Test 1" },
            new TestCustomStructDerived2 { IntProp = 54334, StrProp = "Test 2", IntField = 343, StrField = "Test 3" },
            null,
            new TestCustomStructDerived1 { IntProp = 654546 },
            new TestCustomStructDerived2 { IntProp = 34, IntField = 4545 }
        };

        private static readonly IBaseInterface[] TestPolymorphicArrayOfStructs2 = new IBaseInterface[]
        {
            new TestCustomStructDerived2 { IntProp = 54334, StrProp = "Test 2", IntField = 343, StrField = "Test 3" },
            null,
            new TestCustomStructDerived2 { IntProp = 34, IntField = 4545 }
        };

        private static readonly IBaseInterface[] TestPolymorphicArrayOfClasses1 = new IBaseInterface[]
        {
            new TestCustomClassDerived1 { IntProp = 34234, StrProp = "Test 1", StrProp1 = "Test 2" },
            new TestCustomClassDerived2 { IntField = 575, IntProp = 444, StrProp = "Test 3" },
            null,
            new TestCustomClassDerived3 { IntProp = 454, StrField = "Test 4", StrProp = "Test 5", StrProp1 = "Test 6"},
            new TestCustomClassDerived2 { IntField = 765656, IntProp = 44, StrField = "Test 7", StrProp = "Test 8"}
        };

        private static readonly IBaseInterface[] TestPolymorphicArrayOfClasses2 = new IBaseInterface[]
        {
            new TestCustomClassDerived1 { IntProp = 34234, StrProp = "Test 1", StrProp1 = "Test 2" },
            null,
            new TestCustomClassDerived1 { IntProp = 776, StrProp = "Test 3" }
        };

        private static readonly TestCustomClassAbstract[] TestPolymorphicArrayOfClasses3 = new TestCustomClassAbstract[]
        {
            new TestCustomClassDerived1 { IntProp = 34234, StrProp = "Test 1", StrProp1 = "Test 2" },
            new TestCustomClassDerived3 { IntProp = 454, StrField = "Test 3", StrProp = "Test 4" },
            null,
            new TestCustomClassDerived3 { IntProp = 787666, StrField = "Test 5", StrProp = "Test 6", StrProp1 = "Test 7" },
            new TestCustomClassDerived1 { IntProp = 7667, StrProp = "Test 8", StrProp1 = "Test 9" }
        };

        private static readonly TestCustomClassDerived1[] TestPolymorphicArrayOfClasses4 = new TestCustomClassDerived1[]
        {
            new TestCustomClassDerived1 { IntProp = 34234, StrProp = "Test 1", StrProp1 = "Test 2" },
            new TestCustomClassDerived3 { IntProp = 454, StrField = "Test 3", StrProp = "Test 4" },
            null,
            new TestCustomClassDerived3 { IntProp = 787666, StrField = "Test 5", StrProp = "Test 6", StrProp1 = "Test 7" },
            new TestCustomClassDerived1 { IntProp = 7667, StrProp = "Test 8", StrProp1 = "Test 9" }
        };

        [Fact]
        public void Should_Serialize_Array_Of_Polymorphic_Structs_From_Interface()
        {
            Serializer.RegisterTypes(new[] { typeof(TestCustomStructDerived1), typeof(TestCustomStructDerived2) });
            TestStructField(TestPolymorphicArrayOfStructs1);
            TestStructProperty(TestPolymorphicArrayOfStructs1);
            TestClassField(TestPolymorphicArrayOfStructs1);
            TestClassProperty(TestPolymorphicArrayOfStructs1);
        }

        [Fact]
        public void Should_Serialize_List_Of_Polymorphic_Structs_From_Interface()
        {
            Serializer.RegisterTypes(new[] { typeof(TestCustomStructDerived1), typeof(TestCustomStructDerived2) });
            TestStructField(TestPolymorphicArrayOfStructs1.ToList());
            TestStructProperty(TestPolymorphicArrayOfStructs1.ToList());
            TestClassField(TestPolymorphicArrayOfStructs1.ToList());
            TestClassProperty(TestPolymorphicArrayOfStructs1.ToList());
        }

        [Fact]
        public void Should_Serialize_IEnumerable_From_Array_Of_Polymorphic_Structs_From_Interface()
        {
            Serializer.RegisterTypes(new[] { typeof(TestCustomStructDerived1), typeof(TestCustomStructDerived2) });
            TestStructField<IEnumerable<IBaseInterface>>(TestPolymorphicArrayOfStructs1);
            TestStructProperty<IEnumerable<IBaseInterface>>(TestPolymorphicArrayOfStructs1);
            TestClassField<IEnumerable<IBaseInterface>>(TestPolymorphicArrayOfStructs1);
            TestClassProperty<IEnumerable<IBaseInterface>>(TestPolymorphicArrayOfStructs1);
        }

        [Fact]
        public void Should_Serialize_IEnumerable_From_List_Of_Polymorphic_Structs_From_Interface()
        {
            Serializer.RegisterTypes(new[] { typeof(TestCustomStructDerived1), typeof(TestCustomStructDerived2) });
            TestStructField<IEnumerable<IBaseInterface>>(TestPolymorphicArrayOfStructs1.ToList());
            TestStructProperty<IEnumerable<IBaseInterface>>(TestPolymorphicArrayOfStructs1.ToList());
            TestClassField<IEnumerable<IBaseInterface>>(TestPolymorphicArrayOfStructs1.ToList());
            TestClassProperty<IEnumerable<IBaseInterface>>(TestPolymorphicArrayOfStructs1.ToList());
        }

        [Fact]
        public void Shouldnt_Serialize_Array_Of_Polymorphic_Structs_From_Interface_With_No_Types()
        {
            var message = "No implementations found for";
            var ex = Assert.Throws<NotImplementedException>(() => TestStructField<IEnumerable<IBaseInterface>>(TestPolymorphicArrayOfStructs1.ToList()));
            Assert.StartsWith(message, ex.Message);
            ex = Assert.Throws<NotImplementedException>(() => TestStructProperty<IEnumerable<IBaseInterface>>(TestPolymorphicArrayOfStructs1.ToList()));
            Assert.StartsWith(message, ex.Message);
            ex = Assert.Throws<NotImplementedException>(() => TestClassField<IEnumerable<IBaseInterface>>(TestPolymorphicArrayOfStructs1.ToList()));
            Assert.StartsWith(message, ex.Message);
            ex = Assert.Throws<NotImplementedException>(() => TestClassProperty<IEnumerable<IBaseInterface>>(TestPolymorphicArrayOfStructs1.ToList()));
            Assert.StartsWith(message, ex.Message);
        }

        [Fact]
        public void Shouldnt_Serialize_Array_Of_Polymorphic_Structs_From_Interface()
        {
            var message = "Key not found";
            Serializer.RegisterTypes(new[] { typeof(TestCustomStructDerived1) });
            var ex = Assert.Throws<ArgumentException>(() => TestStructField<IEnumerable<IBaseInterface>>(TestPolymorphicArrayOfStructs1.ToList()));
            Assert.StartsWith(message, ex.Message);
            ex = Assert.Throws<ArgumentException>(() => TestStructProperty<IEnumerable<IBaseInterface>>(TestPolymorphicArrayOfStructs1.ToList()));
            Assert.StartsWith(message, ex.Message);
            ex = Assert.Throws<ArgumentException>(() => TestClassField<IEnumerable<IBaseInterface>>(TestPolymorphicArrayOfStructs1.ToList()));
            Assert.StartsWith(message, ex.Message);
            ex = Assert.Throws<ArgumentException>(() => TestClassProperty<IEnumerable<IBaseInterface>>(TestPolymorphicArrayOfStructs1.ToList()));
            Assert.StartsWith(message, ex.Message);
        }

        [Fact]
        public void Should_Serialize_Array_Of_Polymorphic_Struct_From_Interface()
        {
            Serializer.RegisterTypes(new[] { typeof(TestCustomStructDerived2) });
            TestStructField(TestPolymorphicArrayOfStructs2);
            TestStructProperty(TestPolymorphicArrayOfStructs2);
            TestClassField(TestPolymorphicArrayOfStructs2);
            TestClassProperty(TestPolymorphicArrayOfStructs2);
        }

        [Fact]
        public void Should_Serialize_List_Of_Polymorphic_Struct_From_Interface()
        {
            Serializer.RegisterTypes(new[] { typeof(TestCustomStructDerived2) });
            TestStructField(TestPolymorphicArrayOfStructs2.ToList());
            TestStructProperty(TestPolymorphicArrayOfStructs2.ToList());
            TestClassField(TestPolymorphicArrayOfStructs2.ToList());
            TestClassProperty(TestPolymorphicArrayOfStructs2.ToList());
        }

        [Fact]
        public void Should_Serialize_IEnumerable_From_Array_Of_Polymorphic_Struct_From_Interface()
        {
            Serializer.RegisterTypes(new[] { typeof(TestCustomStructDerived2) });
            TestStructField<IEnumerable<IBaseInterface>>(TestPolymorphicArrayOfStructs2);
            TestStructProperty<IEnumerable<IBaseInterface>>(TestPolymorphicArrayOfStructs2);
            TestClassField<IEnumerable<IBaseInterface>>(TestPolymorphicArrayOfStructs2);
            TestClassProperty<IEnumerable<IBaseInterface>>(TestPolymorphicArrayOfStructs2);
        }

        [Fact]
        public void Should_Serialize_IEnumerable_From_List_Of_Polymorphic_Struct_From_Interface()
        {
            Serializer.RegisterTypes(new[] { typeof(TestCustomStructDerived2) });
            TestStructField<IEnumerable<IBaseInterface>>(TestPolymorphicArrayOfStructs2.ToList());
            TestStructProperty<IEnumerable<IBaseInterface>>(TestPolymorphicArrayOfStructs2.ToList());
            TestClassField<IEnumerable<IBaseInterface>>(TestPolymorphicArrayOfStructs2.ToList());
            TestClassProperty<IEnumerable<IBaseInterface>>(TestPolymorphicArrayOfStructs2.ToList());
        }

        [Fact]
        public void Should_Serialize_Array_Of_Polymorphic_Classes_From_Interface()
        {
            Serializer.RegisterTypes(new[] { typeof(TestCustomClassDerived1), typeof(TestCustomClassDerived2), typeof(TestCustomClassDerived3) });
            TestStructField(TestPolymorphicArrayOfClasses1);
            TestStructProperty(TestPolymorphicArrayOfClasses1);
            TestClassField(TestPolymorphicArrayOfClasses1);
            TestClassProperty(TestPolymorphicArrayOfClasses1);
        }

        [Fact]
        public void Should_Serialize_List_Of_Polymorphic_Classes_From_Interface()
        {
            Serializer.RegisterTypes(new[] { typeof(TestCustomClassDerived1), typeof(TestCustomClassDerived2), typeof(TestCustomClassDerived3) });
            TestStructField(TestPolymorphicArrayOfClasses1.ToList());
            TestStructProperty(TestPolymorphicArrayOfClasses1.ToList());
            TestClassField(TestPolymorphicArrayOfClasses1.ToList());
            TestClassProperty(TestPolymorphicArrayOfClasses1.ToList());
        }

        [Fact]
        public void Should_Serialize_IEnumerable_From_Array_Of_Polymorphic_Classes_From_Interface()
        {
            Serializer.RegisterTypes(new[] { typeof(TestCustomClassDerived1), typeof(TestCustomClassDerived2), typeof(TestCustomClassDerived3) });
            TestStructField<IEnumerable<IBaseInterface>>(TestPolymorphicArrayOfClasses1);
            TestStructProperty<IEnumerable<IBaseInterface>>(TestPolymorphicArrayOfClasses1);
            TestClassField<IEnumerable<IBaseInterface>>(TestPolymorphicArrayOfClasses1);
            TestClassProperty<IEnumerable<IBaseInterface>>(TestPolymorphicArrayOfClasses1);
        }

        [Fact]
        public void Should_Serialize_IEnumerable_From_List_Of_Polymorphic_Classes_From_Interface()
        {
            Serializer.RegisterTypes(new[] { typeof(TestCustomClassDerived1), typeof(TestCustomClassDerived2), typeof(TestCustomClassDerived3) });
            TestStructField<IEnumerable<IBaseInterface>>(TestPolymorphicArrayOfClasses1.ToList());
            TestStructProperty<IEnumerable<IBaseInterface>>(TestPolymorphicArrayOfClasses1.ToList());
            TestClassField<IEnumerable<IBaseInterface>>(TestPolymorphicArrayOfClasses1.ToList());
            TestClassProperty<IEnumerable<IBaseInterface>>(TestPolymorphicArrayOfClasses1.ToList());
        }

        [Fact]
        public void Should_Serialize_Array_Of_Polymorphic_Class_From_Interface()
        {
            Serializer.RegisterTypes(new[] { typeof(TestCustomClassDerived1) });
            TestStructField(TestPolymorphicArrayOfClasses2);
            TestStructProperty(TestPolymorphicArrayOfClasses2);
            TestClassField(TestPolymorphicArrayOfClasses2);
            TestClassProperty(TestPolymorphicArrayOfClasses2);
        }

        [Fact]
        public void Should_Serialize_List_Of_Polymorphic_Class_From_Interface()
        {
            Serializer.RegisterTypes(new[] { typeof(TestCustomClassDerived1) });
            TestStructField(TestPolymorphicArrayOfClasses2.ToList());
            TestStructProperty(TestPolymorphicArrayOfClasses2.ToList());
            TestClassField(TestPolymorphicArrayOfClasses2.ToList());
            TestClassProperty(TestPolymorphicArrayOfClasses2.ToList());
        }

        [Fact]
        public void Should_Serialize_IEnumerable_From_Array_Of_Polymorphic_Class_From_Interface()
        {
            Serializer.RegisterTypes(new[] { typeof(TestCustomClassDerived1) });
            TestStructField<IEnumerable<IBaseInterface>>(TestPolymorphicArrayOfClasses2);
            TestStructProperty<IEnumerable<IBaseInterface>>(TestPolymorphicArrayOfClasses2);
            TestClassField<IEnumerable<IBaseInterface>>(TestPolymorphicArrayOfClasses2);
            TestClassProperty<IEnumerable<IBaseInterface>>(TestPolymorphicArrayOfClasses2);
        }

        [Fact]
        public void Should_Serialize_IEnumerable_From_List_Of_Polymorphic_Class_From_Interface()
        {
            Serializer.RegisterTypes(new[] { typeof(TestCustomClassDerived1) });
            TestStructField<IEnumerable<IBaseInterface>>(TestPolymorphicArrayOfClasses2.ToList());
            TestStructProperty<IEnumerable<IBaseInterface>>(TestPolymorphicArrayOfClasses2.ToList());
            TestClassField<IEnumerable<IBaseInterface>>(TestPolymorphicArrayOfClasses2.ToList());
            TestClassProperty<IEnumerable<IBaseInterface>>(TestPolymorphicArrayOfClasses2.ToList());
        }

        [Fact]
        public void Should_Serialize_Array_Of_Polymorphic_Classes_From_Abstract()
        {
            Serializer.RegisterTypes(new[] { typeof(TestCustomClassDerived1), typeof(TestCustomClassDerived3) });
            TestStructField(TestPolymorphicArrayOfClasses3);
            TestStructProperty(TestPolymorphicArrayOfClasses3);
            TestClassField(TestPolymorphicArrayOfClasses3);
            TestClassProperty(TestPolymorphicArrayOfClasses3);
        }

        [Fact]
        public void Should_Serialize_List_Of_Polymorphic_Classes_From_Abstract()
        {
            Serializer.RegisterTypes(new[] { typeof(TestCustomClassDerived1), typeof(TestCustomClassDerived3) });
            TestStructField(TestPolymorphicArrayOfClasses3.ToList());
            TestStructProperty(TestPolymorphicArrayOfClasses3.ToList());
            TestClassField(TestPolymorphicArrayOfClasses3.ToList());
            TestClassProperty(TestPolymorphicArrayOfClasses3.ToList());
        }

        [Fact]
        public void Should_Serialize_IEnumerable_From_Array_Of_Polymorphic_Classes_From_Abstract()
        {
            Serializer.RegisterTypes(new[] { typeof(TestCustomClassDerived1), typeof(TestCustomClassDerived3) });
            TestStructField<IEnumerable<TestCustomClassAbstract>>(TestPolymorphicArrayOfClasses3);
            TestStructProperty<IEnumerable<TestCustomClassAbstract>>(TestPolymorphicArrayOfClasses3);
            TestClassField<IEnumerable<TestCustomClassAbstract>>(TestPolymorphicArrayOfClasses3);
            TestClassProperty<IEnumerable<TestCustomClassAbstract>>(TestPolymorphicArrayOfClasses3);
        }

        [Fact]
        public void Should_Serialize_IEnumerable_From_List_Of_Polymorphic_Classes_From_Abstract()
        {
            Serializer.RegisterTypes(new[] { typeof(TestCustomClassDerived1), typeof(TestCustomClassDerived3) });
            TestStructField<IEnumerable<TestCustomClassAbstract>>(TestPolymorphicArrayOfClasses3.ToList());
            TestStructProperty<IEnumerable<TestCustomClassAbstract>>(TestPolymorphicArrayOfClasses3.ToList());
            TestClassField<IEnumerable<TestCustomClassAbstract>>(TestPolymorphicArrayOfClasses3.ToList());
            TestClassProperty<IEnumerable<TestCustomClassAbstract>>(TestPolymorphicArrayOfClasses3.ToList());
        }

        [Fact]
        public void Should_Serialize_Array_Of_Polymorphic_Classes_From_Class()
        {
            Serializer.RegisterTypes(new[] { typeof(TestCustomClassDerived1), typeof(TestCustomClassDerived3) });
            TestStructField(TestPolymorphicArrayOfClasses4);
            TestStructProperty(TestPolymorphicArrayOfClasses4);
            TestClassField(TestPolymorphicArrayOfClasses4);
            TestClassProperty(TestPolymorphicArrayOfClasses4);
        }

        [Fact]
        public void Should_Serialize_List_Of_Polymorphic_Classes_From_Class()
        {
            Serializer.RegisterTypes(new[] { typeof(TestCustomClassDerived1), typeof(TestCustomClassDerived3) });
            TestStructField(TestPolymorphicArrayOfClasses4.ToList());
            TestStructProperty(TestPolymorphicArrayOfClasses4.ToList());
            TestClassField(TestPolymorphicArrayOfClasses4.ToList());
            TestClassProperty(TestPolymorphicArrayOfClasses4.ToList());
        }

        [Fact]
        public void Should_Serialize_IEnumerable_From_Array_Of_Polymorphic_Classes_From_Class()
        {
            Serializer.RegisterTypes(new[] { typeof(TestCustomClassDerived1), typeof(TestCustomClassDerived3) });
            TestStructField<IEnumerable<TestCustomClassDerived1>>(TestPolymorphicArrayOfClasses4);
            TestStructProperty<IEnumerable<TestCustomClassDerived1>>(TestPolymorphicArrayOfClasses4);
            TestClassField<IEnumerable<TestCustomClassDerived1>>(TestPolymorphicArrayOfClasses4);
            TestClassProperty<IEnumerable<TestCustomClassDerived1>>(TestPolymorphicArrayOfClasses4);
        }

        [Fact]
        public void Should_Serialize_IEnumerable_From_List_Of_Polymorphic_Classes_From_Class()
        {
            Serializer.RegisterTypes(new[] { typeof(TestCustomClassDerived1), typeof(TestCustomClassDerived3) });
            TestStructField<IEnumerable<TestCustomClassDerived1>>(TestPolymorphicArrayOfClasses4.ToList());
            TestStructProperty<IEnumerable<TestCustomClassDerived1>>(TestPolymorphicArrayOfClasses4.ToList());
            TestClassField<IEnumerable<TestCustomClassDerived1>>(TestPolymorphicArrayOfClasses4.ToList());
            TestClassProperty<IEnumerable<TestCustomClassDerived1>>(TestPolymorphicArrayOfClasses4.ToList());
        }
    }
}
