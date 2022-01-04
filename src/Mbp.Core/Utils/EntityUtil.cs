using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;

namespace Mbp.Utils
{
    /// <summary>
    /// 实体帮助类
    /// </summary>
    public static class EntityUtil
    {
        /// <summary>
        /// 实体浅拷贝
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        public static void EntityCopy<TSource, TDestination>(TSource source, TDestination destination, params string[] ignore)
        {
            foreach (var item in source.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                // 忽略字段
                if (ignore.Contains(item.Name))
                    continue;

                var value = item.GetValue(source);

                if (item.PropertyType == typeof(string) && value == null)
                {
                    continue;
                }

                if (item.PropertyType == typeof(DateTime) && (DateTime)value == DateTime.MinValue)
                {
                    continue;
                }

                item.SetValue(destination, item.GetValue(source, null), null);
            }
        }
    }
}
