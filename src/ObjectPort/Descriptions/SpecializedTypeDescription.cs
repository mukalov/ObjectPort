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
    using Attributes;
    using Common;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Reflection.Emit;

    internal abstract class SpecializedTypeDescription<T> : TypeDescription
    {
#if !NETCORE
        private Lazy<TypeBuilder> _serializerTypeBuilder;
        private Lazy<TypeBuilder> _deserializerTypeBuilder;
#endif

        public Action<T, BinaryWriter> SerializeHanlder;
        public Func<BinaryReader, T> DeserializeHandler;

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

        internal override void Serialize(BinaryWriter writer, object obj)
        {
            SerializeHanlder.Invoke((T)obj, writer);
        }

        internal override object Deserialize(BinaryReader reader)
        {
            return DeserializeHandler(reader);
        }

        internal void Serialize(BinaryWriter writer, T obj)
        {
            SerializeHanlder.Invoke(obj, writer);
        }

        internal override void InitSerializers()
        {
            var instanceExp = Expression.Parameter(typeof(T), "instance");
            var writerExp = Expression.Parameter(typeof(BinaryWriter), "writer");

            var serializerExp = Expression.Lambda<Action<T, BinaryWriter>>(
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
                    SerializeHanlder = (Action<T, BinaryWriter>)Delegate.CreateDelegate(typeof(Action<T, BinaryWriter>), type.GetMethod("InternalSerialize"));
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
            var readerExp = Expression.Parameter(typeof(BinaryReader), "reader");
            var deserializerExp = GetDeserializerExpression(readerExp);
            var lamdaExp = Expression.Lambda<Func<BinaryReader, T>>(deserializerExp, readerExp);

#if !NETCORE
            if (Type.IsPublic || Type.IsNestedPublic)
            {
                try
                {
                    var methodBuilder = _deserializerTypeBuilder.Value.DefineMethod("InternalDeserialize", MethodAttributes.Public | MethodAttributes.Static);
                    lamdaExp.CompileToMethod(methodBuilder);
                    var type = _deserializerTypeBuilder.Value.CreateType();
                    DeserializeHandler = (Func<BinaryReader, T>)Delegate.CreateDelegate(typeof(Func<BinaryReader, T>), type.GetMethod("InternalDeserialize"));
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

        protected virtual IEnumerable<FieldInfo> GetFieldsFilter(IEnumerable<FieldInfo> fields)
        {
            return fields.Where(f => IsFieldSerializable(f));
        }

        protected virtual IEnumerable<PropertyInfo> GetPropertiesFilter(IEnumerable<PropertyInfo> properties)
        {
            return properties.Where(p => IsPropertySerializable(p));
        }

        private static bool IsMemberMarkedAsNonSerializable(MemberInfo pi)
        {
#if NET40
            var attributes = pi.GetCustomAttributes(false);
#else
            var attributes = pi.GetCustomAttributes();
#endif
            return attributes.Any(a => a is NotPortableAttribute
#if !NETCORE
            || a is NonSerializedAttribute
#endif
            );
        }

        private static bool IsFieldSerializable(FieldInfo fi)
        {
            return !IsMemberMarkedAsNonSerializable(fi);
        }

        private static bool IsPropertySerializable(PropertyInfo pi)
        {
            return pi.CanWrite
                && pi.CanRead
                && pi.GetSetMethod()?.IsPublic == true
                && pi.GetGetMethod()?.IsPublic == true
                && !IsMemberMarkedAsNonSerializable(pi);
        }
    }
}
