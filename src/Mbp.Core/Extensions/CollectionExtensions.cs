using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace Mbp.Extensions
{
    public static class CollectionExtensions
    {
        /// <summary>
        /// 如果条件成立，添加项
        /// </summary>
        public static void AddIf<T>(this ICollection<T> collection, T value, bool flag)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            if (flag)
            {
                collection.Add(value);
            }
        }

        /// <summary>
        /// 如果条件成立，添加项
        /// </summary>
        public static void AddIf<T>(this ICollection<T> collection, T value, Func<bool> func)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            if (func())
            {
                collection.Add(value);
            }
        }

        /// <summary>
        /// 如果不存在，添加项
        /// </summary>
        public static void AddIfNotExist<T>(this ICollection<T> collection, T value, Func<T, bool> existFunc = null)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            bool exists = existFunc == null ? collection.Contains(value) : existFunc(value);
            if (!exists)
            {
                collection.Add(value);
            }
        }

        /// <summary>
        /// 如果不为空，添加项
        /// </summary>
        public static void AddIfNotNull<T>(this ICollection<T> collection, T value) where T : class
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            if (value != null)
            {
                collection.Add(value);
            }
        }

        /// <summary>
        /// 获取对象，不存在对使用委托添加对象
        /// </summary>
        public static T GetOrAdd<T>(this ICollection<T> collection, Func<T, bool> selector, Func<T> factory)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            T item = collection.FirstOrDefault(selector);
            if (item == null || (item.GetType().IsValueType && item.Equals(0)))
            {
                item = factory();
                collection.Add(item);
            }

            return item;
        }

        /// <summary>
        /// 判断集合是否为null或空集合
        /// </summary>
        public static bool IsNullOrEmpty<T>(this ICollection<T> collection)
        {
            return collection == null || collection.Count == 0;
        }

        /// <summary>
        /// 转换为用分隔符连接的字符串
        /// </summary>
        /// <typeparam name="T">集合元素类型</typeparam>
        /// <param name="list">集合</param>
        /// <param name="quotes">引号，默认不带引号，范例：单引号 "'"</param>
        /// <param name="separator">分隔符，默认使用逗号分隔</param>
        public static string Join<T>(this IEnumerable<T> list, string quotes = "", string separator = ",")
        {
            if (list == null)
                return string.Empty;
            var result = new StringBuilder();
            foreach (var each in list)
                result.AppendFormat("{0}{1}{0}{2}", quotes, each, separator);
            if (separator == "")
                return result.ToString();
            return result.ToString().TrimEnd(separator.ToCharArray());
        }

        /// <summary>
        /// 如果Key不存在才添加
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void AddIfNotContains<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            if (!dict.ContainsKey(key))
            {
                dict.Add(key, value);
            }
        }

        /// <summary>
        /// 如果Key不存在则添加，否则替换
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            if (!dict.ContainsKey(key))
            {
                dict.Add(key, value);
            }

            dict[key] = value;
        }

        /// <summary>
        /// 如果Key不存在则添加，否则替换
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static void AddOrUpdate<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            dict.AddOrUpdate(key, value, (k, o) => value);
        }

        /// <summary>
        /// 获取指定Key对应数据
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
        {
            TValue result;
            if (!dict.TryGetValue(key, out result))
            {
                return default(TValue);
            }

            return result;
        }

        /// <summary>
        /// 获取或添加
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <param name="valueFactory"></param>
        /// <returns></returns>
        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, Func<TKey, TValue> valueFactory)
        {
            TValue result;
            if (!dict.ContainsKey(key))
            {
                result = valueFactory.Invoke(key);
                dict.Add(key, result);
            }
            else
            {
                result = dict[key];
            }

            return result;
        }

        /// <summary>
        /// 获取或添加
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <param name="valueFactory"></param>
        /// <returns></returns>
        public static TValue GetOrAdd<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dict, TKey key, Func<TKey, TValue> valueFactory)
        {
            return dict.GetOrAdd(key, valueFactory);
        }


        /// <summary>
        ///  转换为动态对象
        /// </summary>
        /// <param name="this">The @this to act on.</param>
        /// <returns>@this as an ExpandoObject.</returns>
        public static ExpandoObject ToExpando(this IDictionary<string, object> @this)
        {
            var expando = new ExpandoObject();
            var expandoDict = (IDictionary<string, object>)expando;

            foreach (var item in @this)
            {
                var d = item.Value as IDictionary<string, object>;
                if (d != null)
                {
                    expandoDict.Add(item.Key, d.ToExpando());
                }
                else
                {
                    expandoDict.Add(item);
                }
            }

            return expando;
        }
    }
}
