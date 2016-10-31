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
    public class OtherEnumerableMembersTests : RegularEnumerableMembersTests
    {
        private static readonly TestCustomStruct[][] TestArrayOfArray = new TestCustomStruct[][]
        {
            new[]
            {
                new TestCustomStruct { IntField = TestIntArray[0], StrField = TestStringArray[0] },
                new TestCustomStruct { IntField = TestIntArray[1], StrField = TestStringArray[1] },
                new TestCustomStruct { IntField = TestIntArray[2], StrField = TestStringArray[2] },
                new TestCustomStruct { IntField = TestIntArray[3], StrField = TestStringArray[3] }
            },
            new[]
            {
                new TestCustomStruct { IntField = TestIntArray[0], StrField = TestStringArray[0] },
                new TestCustomStruct { IntField = TestIntArray[1], StrField = TestStringArray[1] },
                new TestCustomStruct { IntField = TestIntArray[2], StrField = TestStringArray[2] },
            },
            new[]
            {
                new TestCustomStruct { IntField = TestIntArray[0], StrField = TestStringArray[0] },
                new TestCustomStruct { IntField = TestIntArray[1], StrField = TestStringArray[1] },
                new TestCustomStruct { IntField = TestIntArray[2], StrField = TestStringArray[2] },
                new TestCustomStruct { IntField = TestIntArray[3], StrField = TestStringArray[3] }
            }
        };

        [Fact]
        public void Should_Serialize_Array_Of_Array()
        {
            TestStructField(TestArrayOfArray);
            TestStructField(TestArrayOfArray);
            TestStructProperty(TestArrayOfArray);
            TestStructProperty(TestArrayOfArray);
            TestClassField(TestArrayOfArray);
            TestClassField(TestArrayOfArray);
            TestClassProperty(TestArrayOfArray);
            TestClassProperty(TestArrayOfArray);
        }
    }
}
