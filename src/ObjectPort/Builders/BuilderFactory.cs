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
    using System.Reflection;

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

        private static MemberSerializerBuilder CreateBuilder(Type builderType, IEnumerable<Type> argTypes, params object[] constructorArgs)
        {
            try
            {
                return (MemberSerializerBuilder)Activator.CreateInstance(builderType.MakeGenericType(argTypes.ToArray()), constructorArgs);
            }
            catch (TargetInvocationException e)
            {
                if (e.InnerException != null)
                    throw e.InnerException;
                else
                    throw;
            }
        }

        private static MemberSerializerBuilder CreateBuilder(Type builderType, Type argType, params object[] constructorArgs)
        {
            return CreateBuilder(builderType, new[] { argType }, constructorArgs);
        }

        private static TypeDescription GetTypeDescription(Type type, SerializerState state)
        {
            var typeDescription = state.GetDescription(type);
            if (typeDescription == null 
                && !type.IsAbstract 
                && !type.IsInterface 
                && !type.IsBuiltInType() 
                && !type.IsEnumerableType()
                && !type.IsDictionaryType()
                && !type.IsEnum)
                type.TypeNotSupported(type);
            return typeDescription;
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
                        serializerBuilder = CreateBuilder(typeof(PrimitiveNullableBuilder<>), nullableUnderlyingType, nullableUnderlyingType);
                    }
                    else
                    {
                        serializerBuilder = CreateBuilder(typeof(ComplexNullableBuilder<>), nullableUnderlyingType, nullableUnderlyingType, state);
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
                    serializerBuilder = CreateBuilder(typeof(EnumBuilder<>), type, type, Enum.GetUnderlyingType(type));
                }
                else if (type.IsDictionaryType())
                {
                    var dictTypes = type.GetDictionaryArguments();
                    Debug.Assert(dictTypes != null, "Dictionary generic types can't be null for a dictionary");
                    Debug.Assert(state != null, "State can't be null");
                    serializerBuilder = CreateBuilder(
                        typeof(DictionaryBuilder<,,>), 
                        new[] { type, dictTypes.Item1, dictTypes.Item2 }, 
                        type, 
                        dictTypes.Item1, 
                        dictTypes.Item2,
                        GetTypeDescription(dictTypes.Item1, state),
                        GetTypeDescription(dictTypes.Item2, state),
                        state);
                }
                else if (type.IsEnumerableType())
                {
                    var baseElementType = type.GetEnumerableArgument();
                    Debug.Assert(baseElementType != null, "Element generic type can't be null for an enumerable");
                    Debug.Assert(state != null, "State can't be null");
                    serializerBuilder = CreateBuilder(
                        typeof(EnumerableBuilder<,>),
                        new[] { type, baseElementType },
                        type, 
                        baseElementType, 
                        GetTypeDescription(baseElementType, state), 
                        state);
                }
                else if (type.IsInterface || type.IsAbstract)
                {
                    var derivedTypes = state.GetDescriptionsForDerivedTypes(type);
                    if (!derivedTypes.Any())
                        type.NoImplementationsFound(type);

                    if (derivedTypes.Count() > 1)
                    {
                        serializerBuilder = CreateBuilder(typeof(PolymorphicComplexBuilder<>), type, type, state);
                    }
                    else
                    {
                        serializerBuilder = CreateBuilder(typeof(ComplexBuilder<>), derivedTypes.First().Type, derivedTypes.First());
                        serializerBuilder = WrapWithCheckNullBuilder(type, serializerBuilder);
                    }
                }
                else if (nestedTypeDescription != null)
                {
                    var derivedTypes = state.GetDescriptionsForDerivedTypes(type);
                    if (derivedTypes.Count() > 1)
                    {
                        serializerBuilder = CreateBuilder(typeof(PolymorphicComplexBuilder<>), type, type, state);
                    }
                    else
                    {
                        serializerBuilder = CreateBuilder(typeof(ComplexBuilder<>), type, nestedTypeDescription);
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
