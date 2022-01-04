using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;

namespace WuhanIns.Nitrogen.Orm.Dapper.Extensions.Expression2Sql
{
    internal class CacheInfo
    {
        /// <summary>
        /// 属性和真实字段映射
        /// </summary>
        public Dictionary<string, string> DicPropertyField { get; set; }
        /// <summary>
        /// 真实字段和属性映射
        /// </summary>
        public Dictionary<string, string> DicFieldProperty { get; set; }
        /// <summary>
        /// 主键 默认为 ：ID
        /// </summary>
        public string PrimaryKey { get; set; }
        public string TableName { get; set; }
        /// <summary>
        /// 属性列表
        /// </summary>
        public PropertyInfo[] Properties { get; set; }
    }

    public static class PropertyInfoCache
    {
        private static Dictionary<string, CacheInfo> keyValues = new Dictionary<string, CacheInfo>();
        private static object objLock = new object();
        public static void InitCacheInfo<T>()
        {
            var fullName = typeof(T).FullName;
            if (!keyValues.ContainsKey(fullName))
            {
                lock (objLock)
                {
                    if (!keyValues.ContainsKey(fullName))
                    {
                        keyValues.Add(fullName, new CacheInfo
                        {
                            DicPropertyField = ModelCache<T>._DicPropertyField,
                            DicFieldProperty = ModelCache<T>._DicFieldProperty,
                            PrimaryKey = ModelCache<T>._PrimaryKey,
                            TableName = ModelCache<T>._TableName,
                            Properties = ModelCache<T>._Properties
                        });
                    }
                }
            }
        }

        /// <summary>
        /// 获取属性对应的真实字段
        /// </summary>
        /// <param name="funcResult">接收两个参数：property , property 对应的 field,返回新的字符</param>
        /// <param name="propertyNames">属性名称</param>
        /// <returns></returns>
        private static string PropertyNameToFieldName(Dictionary<string, string> DicPropertyField, Dictionary<string, string> DicFieldProperty, Func<string, string, string> funcResult, params string[] propertyNames)
        {
            var result = new List<string>();
            foreach (var propertyName in propertyNames)
            {
                var tempProperty = propertyName.ToUpper().Trim();
                if (DicPropertyField.ContainsKey(tempProperty))
                {
                    result.Add(funcResult(tempProperty, DicPropertyField[tempProperty]));
                }
                else
                {
                    result.Add(tempProperty);
                }
            }
            return string.Join(",", result.ToArray());
        }

        /// <summary>
        /// 获取真实字段并 as 为属性
        /// </summary>
        /// <param name="funcResult">接收两个参数：field , field 对应的 property,返回新的字符</param>
        /// <param name="fields">字段</param>
        /// <returns></returns>
        private static string FieldNameToPropertyName(Dictionary<string, string> DicFieldProperty, Func<string, string, string> funcResult, params string[] fieldNames)
        {
            var result = new List<string>();
            foreach (var field in fieldNames)
            {
                if (string.IsNullOrEmpty(field)) continue;
                var tempField = field.ToUpper().Trim();
                if (DicFieldProperty.ContainsKey(tempField))
                {
                    result.Add(funcResult(tempField, DicFieldProperty[tempField]));
                }
                else
                {
                    result.Add(tempField);
                }
            }
            return string.Join(",", result.ToArray());
        }

        /// <summary>
        /// 获取 字段 as model的属性名
        /// </summary>
        /// <param name="fieldNames"></param>
        /// <returns></returns>
        public static string FieldNameAsPropertyName(string fullName, params string[] fieldNames)
        {
            return FieldNameToPropertyName(keyValues[fullName].DicFieldProperty, (field, property) => string.Format("{0} as {1}", field, property), fieldNames);
        }
        /// <summary>
        /// 获取 字段 as model的属性名
        /// </summary>
        /// <param name="fieldNames"></param>
        /// <returns></returns>
        public static string FieldNameAsPropertyName(string fullName)
        {

            return FieldNameToPropertyName(keyValues[fullName].DicFieldProperty, (field, property) => string.Format("{0} as {1}", field, property), keyValues[fullName].DicPropertyField.Values.ToArray());
        }

        public static string GetTableName(string fullName)
        {
            return keyValues[fullName].TableName;
        }
        public static string GetFieldName(string fullName, string propertyName)
        {
            return PropertyNameToFieldName(keyValues[fullName].DicPropertyField, keyValues[fullName].DicFieldProperty, (p, f) => f, propertyName);
        }

    }
}
