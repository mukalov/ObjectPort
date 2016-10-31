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

namespace ObjectPort.Builders
{
    using Common;
    using Primitive;
    using Descriptions;
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Diagnostics;

    internal class BuilderFactory
    {
        private static readonly Dictionary<Type, MemberSerializerBuilder> PrimitiveBuilders =
            new Dictionary<Type, MemberSerializerBuilder>()
            {
                [typeof(bool)] = new BooleanBuilder(),
                [typeof(byte)] = new ByteBuilder(),
                [typeof(char)] = new CharBuilder(),
                [typeof(decimal)] = new DecimalBuilder(),
                [typeof(double)] = new DoubleBuilder(),
                [typeof(short)] = new ShortBuilder(),
                [typeof(int)] = new IntBuilder(),
                [typeof(long)] = new LongBuilder(),
                [typeof(sbyte)] = new SByteBuilder(),
                [typeof(float)] = new FloatBuilder(),
                [typeof(string)] = new CheckNullBuilder<string>(new StrBuilder()),
                [typeof(ushort)] = new UShortBuilder(),
                [typeof(uint)] = new UIntBuilder(),
                [typeof(ulong)] = new ULongBuilder(),
                [typeof(DateTime)] = new DateTimeBuilder(),
                [typeof(Guid)] = new GuidBuilder(),
                [typeof(TimeSpan)] = new TimeSpanBuilder()
            };

        private static MemberSerializerBuilder WrapWithCheckNullBuilder(Type type, MemberSerializerBuilder builder)
        {
            return (MemberSerializerBuilder)Activator
                .CreateInstance(typeof(CheckNullBuilder<>)
                .MakeGenericType(type), builder);
        }

        internal static MemberSerializerBuilder GetBuilder(Type type, TypeDescription nestedTypeDescription, SerializerState state)
        {
            var serializerBuilder = default(MemberSerializerBuilder);
            if (type.IsBuiltInType())
            {
                var nullableUnderlyingType = Nullable.GetUnderlyingType(type);
                if (nullableUnderlyingType != null)
                {
                    if (nullableUnderlyingType.IsBuiltInType())
                    {
                        serializerBuilder = (MemberSerializerBuilder)Activator
                            .CreateInstance(typeof(PrimitiveNullableBuilder<>)
                            .MakeGenericType(nullableUnderlyingType), nullableUnderlyingType);
                    }
                    else
                    {
                        serializerBuilder = (MemberSerializerBuilder)Activator
                            .CreateInstance(typeof(ComplexNullableBuilder<>)
                            .MakeGenericType(nullableUnderlyingType), nullableUnderlyingType, state);
                    }
                }
                else
                {
                    if (PrimitiveBuilders.TryGetValue(type, out serializerBuilder))
                        return serializerBuilder;
                    type.TypeNotSupported(type);
                }
            }
            else
            {
                if (type.IsEnum)
                {
                    serializerBuilder = (MemberSerializerBuilder)Activator
                        .CreateInstance(typeof(EnumBuilder<>)
                        .MakeGenericType(type), type, Enum.GetUnderlyingType(type));
                }
                else if (type.IsDictionaryType())
                {
                    var dictTypes = type.GetDictionaryArguments();
                    Debug.Assert(dictTypes != null);
                    Debug.Assert(state != null);
                    if (dictTypes.Item2.IsBuiltInType())
                    {
                        serializerBuilder = (MemberSerializerBuilder)Activator
                            .CreateInstance(typeof(DictionaryBuilder<,>)
                            .MakeGenericType(dictTypes.Item1, dictTypes.Item2), type, dictTypes.Item1, dictTypes.Item2, state);
                    }
                    else
                    {
                        // TODO polymorphic dictionary
                    }
                }
                else if (type.IsEnumerableType())
                {
                    var baseElementType = type.GetEnumerableArgument();
                    Debug.Assert(baseElementType != null);
                    Debug.Assert(state != null);
                    if (baseElementType.IsBuiltInType())
                    {
                        serializerBuilder = (MemberSerializerBuilder)Activator
                            .CreateInstance(typeof(EnumerableBuilder<>)
                            .MakeGenericType(baseElementType), type, baseElementType, null, state);
                    }
                    else
                    {
                        var typeDescription = state.GetDescription(baseElementType);
                        if (typeDescription == null && !baseElementType.IsAbstract && !baseElementType.IsInterface)
                            type.TypeNotSupported(baseElementType);

                        try
                        {
                            serializerBuilder = (MemberSerializerBuilder)Activator
                                .CreateInstance(typeof(EnumerableBuilder<>)
                                .MakeGenericType(baseElementType), type, baseElementType, typeDescription, state);
                        }
                        catch (Exception e)
                        {
                            if (e.InnerException != null)
                                throw e.InnerException;
                            else
                                throw;
                        }
                    }
                }
                else if (type.IsInterface || type.IsAbstract)
                {
                    var derivedTypes = state.GetDescriptionsForDerivedTypes(type);
                    if (!derivedTypes.Any())
                        type.NoImplementationsFound(type);

                    if (derivedTypes.Count() > 1)
                    {
                        serializerBuilder = (MemberSerializerBuilder)Activator
                            .CreateInstance(typeof(PolymorphicComplexBuilder<>)
                            .MakeGenericType(type), type, state);
                    }
                    else
                    {
                        serializerBuilder = (MemberSerializerBuilder)Activator
                            .CreateInstance(typeof(ComplexBuilder<>)
                            .MakeGenericType(derivedTypes.First().Type), derivedTypes.First());
                        serializerBuilder = WrapWithCheckNullBuilder(type, serializerBuilder);
                    }
                }
                else if (nestedTypeDescription != null)
                {
                    var derivedTypes = state.GetDescriptionsForDerivedTypes(type);
                    if (derivedTypes.Count() > 1)
                    {
                        serializerBuilder = (MemberSerializerBuilder)Activator
                            .CreateInstance(typeof(PolymorphicComplexBuilder<>)
                            .MakeGenericType(type), type, state);
                    }
                    else
                    {
                        serializerBuilder = (MemberSerializerBuilder)Activator
                            .CreateInstance(typeof(ComplexBuilder<>)
                            .MakeGenericType(type), nestedTypeDescription);
                        if (!type.IsValueType)
                            serializerBuilder = WrapWithCheckNullBuilder(type, serializerBuilder);
                    }
                }
                else
                    type.TypeNotSupported(type);
            }
            return serializerBuilder;
        }
    }
}
