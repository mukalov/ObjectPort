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
    using System.IO;
    using Xunit;

    public class TestsBase : IDisposable
    {
        public class TestClass<T>
        {
            public T Field;
            public T Property { get; set; }
        }

        public struct TestStruct<T>
        {
            public T Field;
            public T Property { get; set; }
        }

        public struct TestCustomStruct
        {
            public string StrField;
            public int IntField;
        }

        public class TestCustomClass
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

        internal delegate void ValueSetter<ContainerT>(ref ContainerT obj);

        public TestsBase()
        {
        }

        public void Dispose()
        {
            Serializer.Clear();
        }

        internal object SerializeDeserializeClass<ContainerT>(Action<ContainerT> setter)
            where ContainerT : new()
        {
            Serializer.RegisterTypes(new[] { typeof(ContainerT) });
            var testObj = new ContainerT();
            setter(testObj);
            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, testObj);
                stream.Seek(0, SeekOrigin.Begin);
                return Serializer.Deserialize(stream);
            }
        }

        internal object SerializeDeserializeStruct<ContainerT>(ValueSetter<ContainerT> setter)
            where ContainerT : new()
        {
            Serializer.RegisterTypes(new[] { typeof(ContainerT) });
            var testObj = new ContainerT();
            setter(ref testObj);
            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, testObj);
                stream.Seek(0, SeekOrigin.Begin);
                var result = Serializer.Deserialize(stream);
                return result;
            }
        }

        internal object SerializeDeserializeValue<T>(T val)
        {
            Serializer.RegisterTypes(new[] { typeof(T) });
            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, val);
                stream.Seek(0, SeekOrigin.Begin);
                var result = Serializer.Deserialize(stream);
                return result;
            }
        }

        private void TestClassMember<ContainerT, T>(T val, Action<ContainerT> setter, Func<ContainerT, T> getter)
            where ContainerT : new()
        {
            var result = SerializeDeserializeClass(setter);
            Assert.IsType<ContainerT>(result);
            Assert.Equal(getter((ContainerT)result), val);
        }

        private void TestStructMember<ContainerT, T>(T val, ValueSetter<ContainerT> setter, Func<ContainerT, T> getter)
            where ContainerT : struct
        {
            var result = SerializeDeserializeStruct(setter);
            Assert.IsType<ContainerT>(result);
            Assert.Equal(getter((ContainerT)result), val);
        }

        internal void TestClassField<T>(T val)
        {
            TestClassMember<TestClass<T>, T>(val, (obj) => { obj.Field = val; }, obj => obj.Field);
        }

        internal void TestClassProperty<T>(T val)
        {
            TestClassMember<TestClass<T>, T>(val, (obj) => { obj.Property = val; }, obj => obj.Property);
        }

        internal void TestStructField<T>(T val)
        {
            TestStructMember<TestStruct<T>, T>(val, (ref TestStruct<T> obj) => { obj.Field = val; }, obj => obj.Field);
        }

        internal void TestStructProperty<T>(T val)
        {
            TestStructMember<TestStruct<T>, T>(val, (ref TestStruct<T> obj) => { obj.Property = val; }, obj => obj.Property);
        }

        internal void TestNoRootObj<T>(T val)
        {
            var result = SerializeDeserializeValue(val);
            Assert.IsType<T>(result);
            Assert.Equal((T)result, val);
        }
    }
}
