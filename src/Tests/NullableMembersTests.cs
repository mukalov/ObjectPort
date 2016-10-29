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
    public class NullableMembersTests : TestsBase
    {
        internal struct TestCustomStruct
        {
            public string StrField;
            public int IntField;
        }

        private static readonly int? TestNullIntVal = -1234343;
        private const int TestIntVal = -4543534;
        private const string TestStringVal = "Test String";


        [Fact]
        public void Should_Serialize_Nullable_Primitive()
        {
            TestStructField(TestNullIntVal);
            TestStructField((int?)null);
            TestStructProperty(TestNullIntVal);
            TestStructProperty((int?)null);
            TestClassField(TestNullIntVal);
            TestClassField((int?)null);
            TestClassProperty(TestNullIntVal);
            TestClassProperty((int?)null);
        }

        [Fact]
        public void Should_Serialize_Nullable_Custom_Struct()
        {
            TestCustomStruct customStruct;
            customStruct.IntField = TestIntVal;
            customStruct.StrField = TestStringVal;
            TestStructField((TestCustomStruct?)customStruct);
            TestStructField((TestCustomStruct?)null);
            TestStructProperty((TestCustomStruct?)customStruct);
            TestStructProperty((TestCustomStruct?)null);
            TestClassField((TestCustomStruct?)customStruct);
            TestClassField((TestCustomStruct?)null);
            TestClassProperty((TestCustomStruct?)customStruct);
            TestClassProperty((TestCustomStruct?)null);
        }
    }
}
