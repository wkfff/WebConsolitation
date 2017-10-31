using System;

namespace Krista.FM.Extensions
{
    public static class TypeExtensions
    {
        public static bool IsNullableType(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static Type GetNonNullableType(this Type type)
        {
            return IsNullableType(type) ? type.GetGenericArguments()[0] : type;
        }

        internal static string GetTypeName(this Type type)
        {
            Type baseType = GetNonNullableType(type);
            string s = baseType.Name;
            if (type != baseType) s += '?';
            return s;
        }

        internal static bool IsEnumType(this Type type)
        {
            return GetNonNullableType(type).IsEnum;
        }

    }
}
