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
    using Xunit;
    using System.Linq;
    using System.Collections.Generic;
    using System;

    [Collection("ObjectPort")]
    public class RegularEnumerableMembersTests : TestsBase
    {
        protected struct TestCustomStruct
        {
            public string StrField;
            public int IntField;
        }

        protected class TestCustomClass
        {
            public string StrField;
            public int IntField;

            public override bool Equals(object obj)
            {
                var testObj = obj as TestCustomClass;
                if (obj == null)
                    return false;
                return StrField == testObj.StrField && IntField == testObj.IntField;
            }

            public override int GetHashCode()
            {
                return (StrField + IntField.ToString()).GetHashCode();
            }
        }

        protected static readonly int[] TestEmptyArray = new int[] { };
        protected static readonly int[] TestNullArray = null;
        protected static readonly List<int> TestNullList = null;
        protected static readonly IEnumerable<int> TestNullEnumerable = null;
        protected static readonly int[] TestIntArray = new[] { -345, 54, -456456, 3453455 };
        protected static readonly int?[] TestNullableIntArray = new int?[] { -345, null, -456456, 3453455 };
        protected static readonly string[] TestStringArray = new[] { "Test string 1", null, "Test string 3", "Test string 4" };
        protected static readonly TestCustomStruct[] TestStructArray = new[]
        {
            new TestCustomStruct { IntField = TestIntArray[0], StrField = TestStringArray[0] },
            new TestCustomStruct { IntField = TestIntArray[1], StrField = TestStringArray[1] },
            new TestCustomStruct { IntField = TestIntArray[2], StrField = TestStringArray[2] },
            new TestCustomStruct { IntField = TestIntArray[3], StrField = TestStringArray[3] }
        };
        private static readonly TestCustomStruct?[] TestNullableStructArray = new TestCustomStruct?[]
        {
            new TestCustomStruct { IntField = TestIntArray[0], StrField = TestStringArray[0] },
            null,
            new TestCustomStruct { IntField = TestIntArray[2], StrField = TestStringArray[2] },
            new TestCustomStruct { IntField = TestIntArray[3], StrField = TestStringArray[3] }
        };
        private static readonly TestCustomClass[] TestClassArray = new[]
        {
            new TestCustomClass { IntField = TestIntArray[0], StrField = TestStringArray[0] },
            null,
            new TestCustomClass { IntField = TestIntArray[2], StrField = TestStringArray[2] },
            new TestCustomClass { IntField = TestIntArray[3], StrField = TestStringArray[3] }
        };

        private enum TestStdEnum { First, Second, Third };
        private static readonly TestStdEnum[] TestEnumArray = new[]
        {
            TestStdEnum.First,
            TestStdEnum.Third,
            TestStdEnum.Second
        };

        [Fact]
        public void Should_Serialize_Empty_Array()
        {
            TestStructField(TestEmptyArray);
            TestStructProperty(TestEmptyArray);
            TestClassField(TestEmptyArray);
            TestClassProperty(TestEmptyArray);
        }

        [Fact]
        public void Should_Serialize_Empty_List()
        {
            TestStructField(TestEmptyArray.ToList());
            TestStructProperty(TestEmptyArray.ToList());
            TestClassField(TestEmptyArray.ToList());
            TestClassProperty(TestEmptyArray.ToList());
        }

        [Fact]
        public void Should_Serialize_Empty_IEnumerable_From_Array()
        {
            TestStructField<IEnumerable<int>>(TestEmptyArray);
            TestStructProperty<IEnumerable<int>>(TestEmptyArray);
            TestClassField<IEnumerable<int>>(TestEmptyArray);
            TestClassProperty<IEnumerable<int>>(TestEmptyArray);
            TestStructField<ICollection<int>>(TestEmptyArray);
            TestStructProperty<ICollection<int>>(TestEmptyArray);
            TestClassField<ICollection<int>>(TestEmptyArray);
            TestClassProperty<ICollection<int>>(TestEmptyArray);
            TestStructField<IList<int>>(TestEmptyArray);
            TestStructProperty<IList<int>>(TestEmptyArray);
            TestClassField<IList<int>>(TestEmptyArray);
            TestClassProperty<IList<int>>(TestEmptyArray);
        }

        [Fact]
        public void Should_Serialize_Empty_IEnumerable_From_List()
        {
            TestStructField<IEnumerable<int>>(TestEmptyArray.ToList());
            TestStructProperty<IEnumerable<int>>(TestEmptyArray.ToList());
            TestClassField<IEnumerable<int>>(TestEmptyArray.ToList());
            TestClassProperty<IEnumerable<int>>(TestEmptyArray.ToList());
            TestStructField<ICollection<int>>(TestEmptyArray.ToList());
            TestStructProperty<ICollection<int>>(TestEmptyArray.ToList());
            TestClassField<ICollection<int>>(TestEmptyArray.ToList());
            TestClassProperty<ICollection<int>>(TestEmptyArray.ToList());
            TestStructField<IList<int>>(TestEmptyArray.ToList());
            TestStructProperty<IList<int>>(TestEmptyArray.ToList());
            TestClassField<IList<int>>(TestEmptyArray.ToList());
            TestClassProperty<IList<int>>(TestEmptyArray.ToList());
        }

        [Fact]
        public void Should_Serialize_Null_Array()
        {
            TestStructField(TestNullArray);
            TestStructProperty(TestNullArray);
            TestClassField(TestNullArray);
            TestClassProperty(TestNullArray);
        }

        [Fact]
        public void Should_Serialize_Null_List()
        {
            TestStructField(TestNullList);
            TestStructProperty(TestNullList);
            TestClassField(TestNullList);
            TestClassProperty(TestNullList);
        }

        [Fact]
        public void Should_Serialize_Null_IEnumerable()
        {
            TestStructField(TestNullEnumerable);
            TestStructProperty(TestNullEnumerable);
            TestClassField(TestNullEnumerable);
            TestClassProperty(TestNullEnumerable);
        }

        [Fact]
        public void Should_Serialize_Array_Of_Primitives()
        {
            TestStructField(TestIntArray);
            TestStructField(TestStringArray);
            TestStructProperty(TestIntArray);
            TestStructProperty(TestStringArray);
            TestClassField(TestIntArray);
            TestClassField(TestStringArray);
            TestClassProperty(TestIntArray);
            TestClassProperty(TestStringArray);
        }

        [Fact]
        public void Should_Serialize_List_Of_Primitives()
        {
            TestStructField(TestIntArray.ToList());
            TestStructField(TestStringArray.ToList());
            TestStructProperty(TestIntArray.ToList());
            TestStructProperty(TestStringArray.ToList());
            TestClassField(TestIntArray.ToList());
            TestClassField(TestStringArray.ToList());
            TestClassProperty(TestIntArray.ToList());
            TestClassProperty(TestStringArray.ToList());
        }

        [Fact]
        public void Should_Serialize_IEnumerable_Of_Primitives_From_Array()
        {
            TestStructField<IEnumerable<int>>(TestIntArray);
            TestStructField<IEnumerable<string>>(TestStringArray);
            TestStructProperty<IEnumerable<int>>(TestIntArray);
            TestStructProperty<IEnumerable<string>>(TestStringArray);
            TestClassField<IEnumerable<int>>(TestIntArray);
            TestClassField<IEnumerable<string>>(TestStringArray);
            TestClassProperty<IEnumerable<int>>(TestIntArray);
            TestClassProperty<IEnumerable<string>>(TestStringArray);
            TestStructField<ICollection<int>>(TestIntArray);
            TestStructField<ICollection<string>>(TestStringArray);
            TestStructProperty<ICollection<int>>(TestIntArray);
            TestStructProperty<ICollection<string>>(TestStringArray);
            TestClassField<ICollection<int>>(TestIntArray);
            TestClassField<ICollection<string>>(TestStringArray);
            TestClassProperty<ICollection<int>>(TestIntArray);
            TestClassProperty<ICollection<string>>(TestStringArray);
            TestStructField<IList<int>>(TestIntArray);
            TestStructField<IList<string>>(TestStringArray);
            TestStructProperty<IList<int>>(TestIntArray);
            TestStructProperty<IList<string>>(TestStringArray);
            TestClassField<IList<int>>(TestIntArray);
            TestClassField<IList<string>>(TestStringArray);
            TestClassProperty<IList<int>>(TestIntArray);
            TestClassProperty<IList<string>>(TestStringArray);
        }

        [Fact]
        public void Should_Serialize_IEnumerable_Of_Primitives_From_List()
        {
            TestStructField<IEnumerable<int>>(TestIntArray.ToList());
            TestStructField<IEnumerable<string>>(TestStringArray.ToList());
            TestStructProperty<IEnumerable<int>>(TestIntArray.ToList());
            TestStructProperty<IEnumerable<string>>(TestStringArray.ToList());
            TestClassField<IEnumerable<int>>(TestIntArray.ToList());
            TestClassField<IEnumerable<string>>(TestStringArray.ToList());
            TestClassProperty<IEnumerable<int>>(TestIntArray.ToList());
            TestClassProperty<IEnumerable<string>>(TestStringArray.ToList());
            TestStructField<ICollection<int>>(TestIntArray.ToList());
            TestStructField<ICollection<string>>(TestStringArray.ToList());
            TestStructProperty<ICollection<int>>(TestIntArray.ToList());
            TestStructProperty<ICollection<string>>(TestStringArray.ToList());
            TestClassField<ICollection<int>>(TestIntArray.ToList());
            TestClassField<ICollection<string>>(TestStringArray.ToList());
            TestClassProperty<ICollection<int>>(TestIntArray.ToList());
            TestClassProperty<ICollection<string>>(TestStringArray.ToList());
            TestStructField<IList<int>>(TestIntArray.ToList());
            TestStructField<IList<string>>(TestStringArray.ToList());
            TestStructProperty<IList<int>>(TestIntArray.ToList());
            TestStructProperty<IList<string>>(TestStringArray.ToList());
            TestClassField<IList<int>>(TestIntArray.ToList());
            TestClassField<IList<string>>(TestStringArray.ToList());
            TestClassProperty<IList<int>>(TestIntArray.ToList());
            TestClassProperty<IList<string>>(TestStringArray.ToList());
        }


        [Fact]
        public void Should_Serialize_Array_Of_Struct()
        {
            TestStructField(TestStructArray);
            TestStructProperty(TestStructArray);
            TestClassField(TestStructArray);
            TestClassProperty(TestStructArray);
        }

        [Fact]
        public void Should_Serialize_List_Of_Struct()
        {
            TestStructField(TestStructArray.ToList());
            TestStructProperty(TestStructArray.ToList());
            TestClassField(TestStructArray.ToList());
            TestClassProperty(TestStructArray.ToList());
        }

        [Fact]
        public void Should_Serialize_IEnumerable_From_Array_Of_Struct()
        {
            TestStructField<IEnumerable<TestCustomStruct>>(TestStructArray);
            TestStructProperty<IEnumerable<TestCustomStruct>>(TestStructArray);
            TestClassField<IEnumerable<TestCustomStruct>>(TestStructArray);
            TestClassProperty<IEnumerable<TestCustomStruct>>(TestStructArray);
            TestStructField<ICollection<TestCustomStruct>>(TestStructArray);
            TestStructProperty<ICollection<TestCustomStruct>>(TestStructArray);
            TestClassField<ICollection<TestCustomStruct>>(TestStructArray);
            TestClassProperty<ICollection<TestCustomStruct>>(TestStructArray);
            TestStructField<IList<TestCustomStruct>>(TestStructArray);
            TestStructProperty<IList<TestCustomStruct>>(TestStructArray);
            TestClassField<IList<TestCustomStruct>>(TestStructArray);
            TestClassProperty<IList<TestCustomStruct>>(TestStructArray);
        }

        [Fact]
        public void Should_Serialize_IEnumerable_From_List_Of_Struct()
        {
            TestStructField<IEnumerable<TestCustomStruct>>(TestStructArray.ToList());
            TestStructProperty<IEnumerable<TestCustomStruct>>(TestStructArray.ToList());
            TestClassField<IEnumerable<TestCustomStruct>>(TestStructArray.ToList());
            TestClassProperty<IEnumerable<TestCustomStruct>>(TestStructArray.ToList());
            TestStructField<ICollection<TestCustomStruct>>(TestStructArray.ToList());
            TestStructProperty<ICollection<TestCustomStruct>>(TestStructArray.ToList());
            TestClassField<ICollection<TestCustomStruct>>(TestStructArray.ToList());
            TestClassProperty<ICollection<TestCustomStruct>>(TestStructArray.ToList());
            TestStructField<IList<TestCustomStruct>>(TestStructArray.ToList());
            TestStructProperty<IList<TestCustomStruct>>(TestStructArray.ToList());
            TestClassField<IList<TestCustomStruct>>(TestStructArray.ToList());
            TestClassProperty<IList<TestCustomStruct>>(TestStructArray.ToList());
        }

        [Fact]
        public void Should_Serialize_Array_Of_Class()
        {
            TestStructField(TestClassArray);
            TestStructProperty(TestClassArray);
            TestClassField(TestClassArray);
            TestClassProperty(TestClassArray);
        }

        [Fact]
        public void Should_Serialize_List_Of_Class()
        {
            TestStructField(TestClassArray.ToList());
            TestStructProperty(TestClassArray.ToList());
            TestClassField(TestClassArray.ToList());
            TestClassProperty(TestClassArray.ToList());
        }

        [Fact]
        public void Should_Serialize_IEnumerable_From_Array_Of_Class()
        {
            TestStructField<IEnumerable<TestCustomClass>>(TestClassArray);
            TestStructProperty<IEnumerable<TestCustomClass>>(TestClassArray);
            TestClassField<IEnumerable<TestCustomClass>>(TestClassArray);
            TestClassProperty<IEnumerable<TestCustomClass>>(TestClassArray);
            TestStructField<ICollection<TestCustomClass>>(TestClassArray);
            TestStructProperty<ICollection<TestCustomClass>>(TestClassArray);
            TestClassField<ICollection<TestCustomClass>>(TestClassArray);
            TestClassProperty<ICollection<TestCustomClass>>(TestClassArray);
            TestStructField<IList<TestCustomClass>>(TestClassArray);
            TestStructProperty<IList<TestCustomClass>>(TestClassArray);
            TestClassField<IList<TestCustomClass>>(TestClassArray);
            TestClassProperty<IList<TestCustomClass>>(TestClassArray);
        }

        [Fact]
        public void Should_Serialize_IEnumerable_From_List_Of_Class()
        {
            TestStructField<IEnumerable<TestCustomClass>>(TestClassArray.ToList());
            TestStructProperty<IEnumerable<TestCustomClass>>(TestClassArray.ToList());
            TestClassField<IEnumerable<TestCustomClass>>(TestClassArray.ToList());
            TestClassProperty<IEnumerable<TestCustomClass>>(TestClassArray.ToList());
            TestStructField<ICollection<TestCustomClass>>(TestClassArray.ToList());
            TestStructProperty<ICollection<TestCustomClass>>(TestClassArray.ToList());
            TestClassField<ICollection<TestCustomClass>>(TestClassArray.ToList());
            TestClassProperty<ICollection<TestCustomClass>>(TestClassArray.ToList());
            TestStructField<IList<TestCustomClass>>(TestClassArray.ToList());
            TestStructProperty<IList<TestCustomClass>>(TestClassArray.ToList());
            TestClassField<IList<TestCustomClass>>(TestClassArray.ToList());
            TestClassProperty<IList<TestCustomClass>>(TestClassArray.ToList());
        }

        [Fact]
        public void Should_Serialize_Array_Of_Nullable_Primitives()
        {
            TestStructField(TestNullableIntArray);
            TestStructProperty(TestNullableIntArray);
            TestClassField(TestNullableIntArray);
            TestClassProperty(TestNullableIntArray);
        }

        [Fact]
        public void Should_Serialize_List_Of_Nullable_Primitives()
        {
            TestStructField(TestNullableIntArray.ToList());
            TestStructProperty(TestNullableIntArray.ToList());
            TestClassField(TestNullableIntArray.ToList());
            TestClassProperty(TestNullableIntArray.ToList());
        }

        [Fact]
        public void Should_Serialize_IEnumerable_From_Array_Of_Nullable_Primitives()
        {
            TestStructField<IEnumerable<int?>>(TestNullableIntArray);
            TestStructProperty<IEnumerable<int?>>(TestNullableIntArray);
            TestClassField<IEnumerable<int?>>(TestNullableIntArray);
            TestClassProperty<IEnumerable<int?>>(TestNullableIntArray);
            TestStructField<ICollection<int?>>(TestNullableIntArray);
            TestStructProperty<ICollection<int?>>(TestNullableIntArray);
            TestClassField<ICollection<int?>>(TestNullableIntArray);
            TestClassProperty<ICollection<int?>>(TestNullableIntArray);
            TestStructField<IList<int?>>(TestNullableIntArray);
            TestStructProperty<IList<int?>>(TestNullableIntArray);
            TestClassField<IList<int?>>(TestNullableIntArray);
            TestClassProperty<IList<int?>>(TestNullableIntArray);
        }

        [Fact]
        public void Should_Serialize_IEnumerable_From_List_Of_Nullable_Primitives()
        {
            TestStructField<IEnumerable<int?>>(TestNullableIntArray.ToList());
            TestStructProperty<IEnumerable<int?>>(TestNullableIntArray.ToList());
            TestClassField<IEnumerable<int?>>(TestNullableIntArray.ToList());
            TestClassProperty<IEnumerable<int?>>(TestNullableIntArray.ToList());
            TestStructField<ICollection<int?>>(TestNullableIntArray.ToList());
            TestStructProperty<ICollection<int?>>(TestNullableIntArray.ToList());
            TestClassField<ICollection<int?>>(TestNullableIntArray.ToList());
            TestClassProperty<ICollection<int?>>(TestNullableIntArray.ToList());
            TestStructField<IList<int?>>(TestNullableIntArray.ToList());
            TestStructProperty<IList<int?>>(TestNullableIntArray.ToList());
            TestClassField<IList<int?>>(TestNullableIntArray.ToList());
            TestClassProperty<IList<int?>>(TestNullableIntArray.ToList());
        }

        [Fact]
        public void Should_Serialize_Array_Of_Nullable_Custom()
        {
            TestStructField(TestNullableStructArray);
            TestStructProperty(TestNullableStructArray);
            TestClassField(TestNullableStructArray);
            TestClassProperty(TestNullableStructArray);
        }

        [Fact]
        public void Should_Serialize_List_Of_Nullable_Custom()
        {
            TestStructField(TestNullableStructArray.ToList());
            TestStructProperty(TestNullableStructArray.ToList());
            TestClassField(TestNullableStructArray.ToList());
            TestClassProperty(TestNullableStructArray.ToList());
        }

        [Fact]
        public void Should_Serialize_IEnumerable_From_Array_Of_Nullable_Custom()
        {
            TestStructField<IEnumerable<TestCustomStruct?>>(TestNullableStructArray);
            TestStructProperty<IEnumerable<TestCustomStruct?>>(TestNullableStructArray);
            TestClassField<IEnumerable<TestCustomStruct?>>(TestNullableStructArray);
            TestClassProperty<IEnumerable<TestCustomStruct?>>(TestNullableStructArray);
            TestStructField<ICollection<TestCustomStruct?>>(TestNullableStructArray);
            TestStructProperty<ICollection<TestCustomStruct?>>(TestNullableStructArray);
            TestClassField<ICollection<TestCustomStruct?>>(TestNullableStructArray);
            TestClassProperty<ICollection<TestCustomStruct?>>(TestNullableStructArray);
            TestStructField<IList<TestCustomStruct?>>(TestNullableStructArray);
            TestStructProperty<IList<TestCustomStruct?>>(TestNullableStructArray);
            TestClassField<IList<TestCustomStruct?>>(TestNullableStructArray);
            TestClassProperty<IList<TestCustomStruct?>>(TestNullableStructArray);
        }

        [Fact]
        public void Should_Serialize_IEnumerable_From_List_Of_Nullable_Custom()
        {
            TestStructField<IEnumerable<TestCustomStruct?>>(TestNullableStructArray.ToList());
            TestStructProperty<IEnumerable<TestCustomStruct?>>(TestNullableStructArray.ToList());
            TestClassField<IEnumerable<TestCustomStruct?>>(TestNullableStructArray.ToList());
            TestClassProperty<IEnumerable<TestCustomStruct?>>(TestNullableStructArray.ToList());
            TestStructField<ICollection<TestCustomStruct?>>(TestNullableStructArray.ToList());
            TestStructProperty<ICollection<TestCustomStruct?>>(TestNullableStructArray.ToList());
            TestClassField<ICollection<TestCustomStruct?>>(TestNullableStructArray.ToList());
            TestClassProperty<ICollection<TestCustomStruct?>>(TestNullableStructArray.ToList());
            TestStructField<IList<TestCustomStruct?>>(TestNullableStructArray.ToList());
            TestStructProperty<IList<TestCustomStruct?>>(TestNullableStructArray.ToList());
            TestClassField<IList<TestCustomStruct?>>(TestNullableStructArray.ToList());
            TestClassProperty<IList<TestCustomStruct?>>(TestNullableStructArray.ToList());
        }

        [Fact]
        public void Should_Serialize_Array_Of_Enum()
        {
            TestStructField(TestEnumArray);
            TestStructProperty(TestEnumArray);
            TestClassField(TestEnumArray);
            TestClassProperty(TestEnumArray);
        }

        [Fact]
        public void Should_Serialize_List_Of_Enum()
        {
            TestStructField(TestEnumArray.ToList());
            TestStructProperty(TestEnumArray.ToList());
            TestClassField(TestEnumArray.ToList());
            TestClassProperty(TestEnumArray.ToList());
        }

        [Fact]
        public void Should_Serialize_IEnumerable_From_Array_Of_Enum()
        {
            TestStructField<IEnumerable<TestStdEnum>>(TestEnumArray);
            TestStructProperty<IEnumerable<TestStdEnum>>(TestEnumArray);
            TestClassField<IEnumerable<TestStdEnum>>(TestEnumArray);
            TestClassProperty<IEnumerable<TestStdEnum>>(TestEnumArray);
            TestStructField<ICollection<TestStdEnum>>(TestEnumArray);
            TestStructProperty<ICollection<TestStdEnum>>(TestEnumArray);
            TestClassField<ICollection<TestStdEnum>>(TestEnumArray);
            TestClassProperty<ICollection<TestStdEnum>>(TestEnumArray);
            TestStructField<IList<TestStdEnum>>(TestEnumArray);
            TestStructProperty<IList<TestStdEnum>>(TestEnumArray);
            TestClassField<IList<TestStdEnum>>(TestEnumArray);
            TestClassProperty<IList<TestStdEnum>>(TestEnumArray);
        }

        [Fact]
        public void Should_Serialize_IEnumerable_From_List_Of_Enum()
        {
            TestStructField<IEnumerable<TestStdEnum>>(TestEnumArray.ToList());
            TestStructProperty<IEnumerable<TestStdEnum>>(TestEnumArray.ToList());
            TestClassField<IEnumerable<TestStdEnum>>(TestEnumArray.ToList());
            TestClassProperty<IEnumerable<TestStdEnum>>(TestEnumArray.ToList());
            TestStructField<ICollection<TestStdEnum>>(TestEnumArray.ToList());
            TestStructProperty<ICollection<TestStdEnum>>(TestEnumArray.ToList());
            TestClassField<ICollection<TestStdEnum>>(TestEnumArray.ToList());
            TestClassProperty<ICollection<TestStdEnum>>(TestEnumArray.ToList());
            TestStructField<IList<TestStdEnum>>(TestEnumArray.ToList());
            TestStructProperty<IList<TestStdEnum>>(TestEnumArray.ToList());
            TestClassField<IList<TestStdEnum>>(TestEnumArray.ToList());
            TestClassProperty<IList<TestStdEnum>>(TestEnumArray.ToList());
        }
    }
}
