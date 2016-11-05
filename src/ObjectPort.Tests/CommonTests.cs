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
    using System.IO;
    using Xunit;

#if !NET40
    [Collection("ObjectPort")]
#endif
    public class CommonTests : TestsBase
    {
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

        }

        [Fact]
        public void Should_Serialize_Type_By_Id()
        {
        }

        [Fact]
        public void Shouldnt_Serialize_Unknown_Root()
        {
        }
    }
}
