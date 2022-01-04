using System;
using System.Linq;
using System.Reflection;

namespace Mbp.Utils
{
    public static class InternalTypeUtil
    {
        /// <summary>
        /// 是否是Func<>
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsFunc(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var type = obj.GetType();
            if (!type.GetTypeInfo().IsGenericType)
            {
                return false;
            }

            return type.GetGenericTypeDefinition() == typeof(Func<>);
        }

        /// <summary>
        /// 是否是Func<>
        /// </summary>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsFunc<TReturn>(object obj)
        {
            return obj is Func<TReturn>;
        }

        /// <summary>
        /// 是否是扩展自Nullable<>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="includeEnums"></param>
        /// <returns></returns>
        public static bool IsPrimitiveExtendedIncludingNullable(Type type, bool includeEnums = false)
        {
            if (IsPrimitiveExtended(type, includeEnums))
            {
                return true;
            }

            if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return IsPrimitiveExtended(type.GenericTypeArguments[0], includeEnums);
            }

            return false;
        }

        /// <summary>
        /// 是否是扩展自Nullable<>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="includeNullables"></param>
        /// <param name="includeEnums"></param>
        /// <returns></returns>
        public static bool IsPrimitiveExtended(Type type, bool includeNullables = true, bool includeEnums = false)
        {
            if (IsPrimitiveExtendedInternal(type, includeEnums))
            {
                return true;
            }

            if (includeNullables && IsNullable(type) && type.GenericTypeArguments.Any())
            {
                return IsPrimitiveExtendedInternal(type.GenericTypeArguments[0], includeEnums);
            }

            return false;
        }

        /// <summary>
        /// 是否是扩展自Nullable<>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="includeEnums"></param>
        /// <returns></returns>
        private static bool IsPrimitiveExtended(Type type, bool includeEnums)
        {
            if (type.GetTypeInfo().IsPrimitive)
            {
                return true;
            }

            if (includeEnums && type.GetTypeInfo().IsEnum)
            {
                return true;
            }

            return type == typeof(string) ||
                   type == typeof(decimal) ||
                   type == typeof(DateTime) ||
                   type == typeof(DateTimeOffset) ||
                   type == typeof(TimeSpan) ||
                   type == typeof(Guid);
        }

        /// <summary>
        /// 是否是扩展自Nullable<>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="includeEnums"></param>
        /// <returns></returns>
        private static bool IsPrimitiveExtendedInternal(Type type, bool includeEnums)
        {
            if (type.IsPrimitive)
            {
                return true;
            }

            if (includeEnums && type.IsEnum)
            {
                return true;
            }

            return type == typeof(string) ||
                   type == typeof(decimal) ||
                   type == typeof(DateTime) ||
                   type == typeof(DateTimeOffset) ||
                   type == typeof(TimeSpan) ||
                   type == typeof(Guid);
        }

        /// <summary>
        /// 是否是Nullable<>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsNullable(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
    }
}
