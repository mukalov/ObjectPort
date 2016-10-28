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
    using System.Collections.Generic;
    using Xunit;

    public class DictionaryMembersTests : TestsBase
    {
        private static readonly Dictionary<int, string> TestEmptyDictionary = new Dictionary<int, string>();
        private static readonly Dictionary<int, string> TestNullDictionary = null;
        private static readonly Dictionary<int, string> TestDictionary1 = new Dictionary<int, string>()
        {
            [1] = "test1",
            [2] = "test2",
            [3] = null,
            [4] = "test4"
        };

        [Fact]
        public void Should_Serialize_Empty_Dictionary()
        {
            TestStructField(TestEmptyDictionary);
            TestStructProperty(TestEmptyDictionary);
            TestClassField(TestEmptyDictionary);
            TestClassProperty(TestEmptyDictionary);
        }

        [Fact]
        public void Should_Serialize_Empty_IDictionary()
        {
            TestStructField<IDictionary<int, string>>(TestEmptyDictionary);
            TestStructProperty<IDictionary<int, string>>(TestEmptyDictionary);
            TestClassField<IDictionary<int, string>>(TestEmptyDictionary);
            TestClassProperty<IDictionary<int, string>>(TestEmptyDictionary);
        }

        [Fact]
        public void Should_Serialize_Null_Dictionary()
        {
            TestStructField(TestNullDictionary);
            TestStructProperty(TestNullDictionary);
            TestClassField(TestNullDictionary);
            TestClassProperty(TestNullDictionary);
        }

        [Fact]
        public void Should_Serialize_Null_IDictionary()
        {
            TestStructField<IDictionary<int, string>>(TestNullDictionary);
            TestStructProperty<IDictionary<int, string>>(TestNullDictionary);
            TestClassField<IDictionary<int, string>>(TestNullDictionary);
            TestClassProperty<IDictionary<int, string>>(TestNullDictionary);
        }

        [Fact]
        public void Should_Serialize_Dictionary_Of_Strings()
        {
            TestStructField(TestDictionary1);
            TestStructProperty(TestDictionary1);
            TestClassField(TestDictionary1);
            TestClassProperty(TestDictionary1);
        }

        [Fact]
        public void Should_Serialize_IDictionary_Of_Strings()
        {
            TestStructField<IDictionary<int, string>>(TestDictionary1);
            TestStructProperty<IDictionary<int, string>>(TestDictionary1);
            TestClassField<IDictionary<int, string>>(TestDictionary1);
            TestClassProperty<IDictionary<int, string>>(TestDictionary1);
        }
    }
}
