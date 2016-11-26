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

namespace ObjectPort.Common
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    internal static class TypeExtensions
    {
#if NET40
        internal static Type GetTypeInfo(this Type type)
        {
            return type;
        }
#endif

        internal static bool IsAnonymousType(this Type type)
        {
            Debug.Assert(type != null, "Type can't be null");
            return type.GetTypeInfo().IsDefined(typeof(CompilerGeneratedAttribute), false)
                && type.GetTypeInfo().IsGenericType && type.Name.Contains("AnonymousType")
                && (type.Name.StartsWith("<>", StringComparison.OrdinalIgnoreCase) || type.Name.StartsWith("VB$", StringComparison.OrdinalIgnoreCase))
                && (type.GetTypeInfo().Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic;
        }

        internal static bool IsEnumerableType(this Type type)
        {
            Debug.Assert(type != null, "Type can't be null");
            return
                type.GetTypeInfo().IsArray 
                || type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>) 
                || !"System".Equals(type.Namespace) 
                && type
                .GetTypeInfo().GetInterfaces()
                .Any(i => i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
        }

        internal static bool IsDictionaryType(this Type type)
        {
            Debug.Assert(type != null, "Type can't be null");
            return
                type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(IDictionary<,>)
                || !"System".Equals(type.Namespace)
                && type
                .GetTypeInfo().GetInterfaces()
                .Any(i => i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition() == typeof(IDictionary<,>));
        }

        internal static bool IsBuiltInType(this Type type)
        {
            Debug.Assert(type != null, "Type can't be null");
            return "System".Equals(type.Namespace) && !type.IsEnumerableType();
        }

        internal static bool IsNotComplexType(this Type type)
        {
            Debug.Assert(type != null, "Type can't be null");
            return type.IsBuiltInType()
                || type.IsEnumerableType()
                || type.IsDictionaryType()
                || type.GetTypeInfo().IsEnum;
        }

        internal static Type GetEnumerableArgument(this Type type)
        {
            Debug.Assert(type != null, "Type can't be null");
            if (!type.IsEnumerableType())
                return null;
            Type argType;
            if (type.GetTypeInfo().IsArray)
                argType = type.GetElementType();
            else
            {
#if NET40
                var args = type.GetGenericArguments();
#else
                var args = type.GenericTypeArguments;
#endif
                Debug.Assert(args != null && args.Count() == 1, "Generic types can't be null or empty for a generic type");
                argType = args[0];
            }

            return argType;
        }

        internal static Tuple<Type, Type> GetDictionaryArguments(this Type type)
        {
            Debug.Assert(type != null, "Type can't be null");
            if (!type.IsDictionaryType())
                return null;

#if NET40
            var args = type.GetGenericArguments();
#else
            var args = type.GenericTypeArguments;
#endif
            Debug.Assert(args != null && args.Count() == 2);
            return new Tuple<Type, Type>(args[0], args[1]);
        }
    }
}
