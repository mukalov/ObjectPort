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

namespace ObjectPort.Descriptions
{
    using Common;
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Reflection.Emit;

    internal abstract class SpecializedTypeDescription<T> : TypeDescription
    {
#if !NETCORE
        private Lazy<TypeBuilder> _serializerTypeBuilder;
        private Lazy<TypeBuilder> _deserializerTypeBuilder;
#endif

        public Action<T, Writer> SerializeHanlder;
        public Func<Reader, T> DeserializeHandler;

        internal SpecializedTypeDescription(ushort typeId, Type type, SerializerState state)
            : base(typeId, type, state)
        {
#if !NETCORE
            var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
                new AssemblyName("ObjectPort.InternalSerialization"),
                AssemblyBuilderAccess.Run);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule("InternalSerializerModule");

            _serializerTypeBuilder = new Lazy<TypeBuilder>(() =>
            {
                return moduleBuilder.DefineType("InternalSerializer", TypeAttributes.Public);
            });
            _deserializerTypeBuilder = new Lazy<TypeBuilder>(() =>
            {
                return moduleBuilder.DefineType("InternalDeserializer", TypeAttributes.Public);
            });
#endif
        }

        internal abstract MemberDescription[] GetDescriptions(SerializerState state);

        internal override void Serialize(Writer writer, object obj)
        {
            SerializeHanlder.Invoke((T)obj, writer);
        }

        internal override object Deserialize(Reader reader)
        {
            return DeserializeHandler(reader);
        }

        internal void Serialize(Writer writer, T obj)
        {
            SerializeHanlder.Invoke(obj, writer);
        }

        internal override void InitSerializers()
        {
            var instanceExp = Expression.Parameter(typeof(T), "instance");
            var writerExp = Expression.Parameter(typeof(Writer), "writer");

            var serializerExp = Expression.Lambda<Action<T, Writer>>(
                GetSerializerExpression(instanceExp, writerExp),
                instanceExp,
                writerExp);

#if !NETCORE
            if (Type.IsPublic || Type.IsNestedPublic)
            {
                try
                {
                    var methodBuilder = _serializerTypeBuilder.Value.DefineMethod("InternalSerialize", MethodAttributes.Public | MethodAttributes.Static);
                    serializerExp.CompileToMethod(methodBuilder);
                    var type = _serializerTypeBuilder.Value.CreateType();
                    SerializeHanlder = (Action<T, Writer>)Delegate.CreateDelegate(typeof(Action<T, Writer>), type.GetMethod("InternalSerialize"));
                }
                catch (Exception)
                {
                    SerializeHanlder = serializerExp.Compile();
                }
            }
            else
#endif
                SerializeHanlder = serializerExp.Compile();
        }

        internal override void InitDeserializer()
        {
            var readerExp = Expression.Parameter(typeof(Reader), "reader");
            var deserializerExp = GetDeserializerExpression(readerExp);
            var lamdaExp = Expression.Lambda<Func<Reader, T>>(deserializerExp, readerExp);

#if !NETCORE
            if (Type.IsPublic || Type.IsNestedPublic)
            {
                try
                {
                    var methodBuilder = _deserializerTypeBuilder.Value.DefineMethod("InternalDeserialize", MethodAttributes.Public | MethodAttributes.Static);
                    lamdaExp.CompileToMethod(methodBuilder);
                    var type = _deserializerTypeBuilder.Value.CreateType();
                    DeserializeHandler = (Func<Reader, T>)Delegate.CreateDelegate(typeof(Func<Reader, T>), type.GetMethod("InternalDeserialize"));
                }
                catch (Exception)
                {
                    DeserializeHandler = lamdaExp.Compile();
                }
            }
            else
#endif
                DeserializeHandler = lamdaExp.Compile();
        }
    }
}
