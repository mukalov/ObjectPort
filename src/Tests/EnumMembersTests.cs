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

    [Collection("ObjectPort")]
    public class EnumMembersTests : TestsBase
    {
        private enum TestStdEnum { First, Second, Third };
        private enum TestDerivedEnum : byte { FirstByte, SecondByte, ThirdByte };

        private const TestStdEnum TestStdEnumVal = TestStdEnum.Second;
        private const TestDerivedEnum TestDerivedEnumVal = TestDerivedEnum.SecondByte;
        
        [Fact]
        public void Should_Serialize_Class_Field()
        {
            TestClassField(TestStdEnumVal);
            TestClassField(TestDerivedEnumVal);
        }

        [Fact]
        public void Should_Serialize_Class_Property()
        {
            TestClassProperty(TestStdEnumVal);
            TestClassProperty(TestDerivedEnumVal);
        }

        [Fact]
        public void Should_Serialize_Struct_Field()
        {
            TestStructField(TestStdEnumVal);
            TestStructField(TestDerivedEnumVal);
        }

        [Fact]
        public void Should_Serialize_Struct_Property()
        {
            TestStructProperty(TestStdEnumVal);
            TestStructProperty(TestDerivedEnumVal);
        }
    }
}
