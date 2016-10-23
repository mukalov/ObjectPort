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
    using Xunit;

    public class PrimitiveMembersTests : TestsBase
    {
        private const int TestIntVal = -1234343;
        private const bool TestBoolVal = true;
        private const byte TestByteVal = 25;
        private const char TestCharVal = 'A';
        private static readonly DateTime TestDateTimeVal = DateTime.Now;
        private const decimal TestDecimalVal = 22.46M;
        private const double TestDoubleVal = 2344.56765;
        private const float TestFloatVal = 2343.5645F;
        private const long TestLongVal = -4356544654556555546;
        private const sbyte TestSByteVal = -45;
        private const short TestShortVal = -32434;
        private const string TestStringVal = "Test String";
        private const uint TestUintVal = 3534534534;
        private const ulong TestULongVal = 8234423423564235234;
        private const ushort TestUShortVal = 56545;
        private static readonly Guid TestGuidVal = Guid.NewGuid();
        private static readonly TimeSpan TestSpanVal = TimeSpan.FromHours(4);


        [Fact]
        public void Should_Serialize_Class_Field()
        {
            TestClassField(TestIntVal);
            TestClassField(TestBoolVal);
            TestClassField(TestByteVal);
            TestClassField(TestCharVal);
            TestClassField(TestDateTimeVal);
            TestClassField(TestDecimalVal);
            TestClassField(TestDoubleVal);
            TestClassField(TestFloatVal);
            TestClassField(TestLongVal);
            TestClassField(TestSByteVal);
            TestClassField(TestShortVal);
            TestClassField(TestStringVal);
            TestClassField(TestUintVal);
            TestClassField(TestULongVal);
            TestClassField(TestUShortVal);
            TestClassField(TestGuidVal);
            TestClassField(TestSpanVal);
        }

        [Fact]
        public void Should_Serialize_Class_Property()
        {
            TestClassProperty(TestIntVal);
            TestClassProperty(TestBoolVal);
            TestClassProperty(TestByteVal);
            TestClassProperty(TestCharVal);
            TestClassProperty(TestDateTimeVal);
            TestClassProperty(TestDecimalVal);
            TestClassProperty(TestDoubleVal);
            TestClassProperty(TestFloatVal);
            TestClassProperty(TestLongVal);
            TestClassProperty(TestSByteVal);
            TestClassProperty(TestShortVal);
            TestClassProperty(TestStringVal);
            TestClassProperty(TestUintVal);
            TestClassProperty(TestULongVal);
            TestClassProperty(TestUShortVal);
            TestClassProperty(TestGuidVal);
            TestClassProperty(TestSpanVal);
        }

        [Fact]
        public void Should_Serialize_Struct_Field()
        {
            TestStructField(TestIntVal);
            TestStructField(TestBoolVal);
            TestStructField(TestByteVal);
            TestStructField(TestCharVal);
            TestStructField(TestDateTimeVal);
            TestStructField(TestDecimalVal);
            TestStructField(TestDoubleVal);
            TestStructField(TestFloatVal);
            TestStructField(TestLongVal);
            TestStructField(TestSByteVal);
            TestStructField(TestShortVal);
            TestStructField(TestStringVal);
            TestStructField(TestUintVal);
            TestStructField(TestULongVal);
            TestStructField(TestUShortVal);
            TestStructField(TestGuidVal);
            TestStructField(TestSpanVal);
        }

        [Fact]
        public void Should_Serialize_Struct_Property()
        {
            TestStructProperty(TestIntVal);
            TestStructProperty(TestBoolVal);
            TestStructProperty(TestByteVal);
            TestStructProperty(TestCharVal);
            TestStructProperty(TestDateTimeVal);
            TestStructProperty(TestDecimalVal);
            TestStructProperty(TestDoubleVal);
            TestStructProperty(TestFloatVal);
            TestStructProperty(TestLongVal);
            TestStructProperty(TestSByteVal);
            TestStructProperty(TestShortVal);
            TestStructProperty(TestStringVal);
            TestStructProperty(TestUintVal);
            TestStructProperty(TestULongVal);
            TestStructProperty(TestUShortVal);
            TestStructProperty(TestGuidVal);
            TestStructProperty(TestSpanVal);
        }

        [Fact]
        public void Should_Serialize_Default_Value()
        {
            TestStructField<string>(null);
            TestStructProperty<string>(null);
            TestClassField<string>(null);
            TestClassProperty<string>(null);
        }
    }
}
