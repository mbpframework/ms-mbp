using WuhanIns.Nitrogen.Orm.Dapper.Extensions.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;

namespace WuhanIns.Nitrogen.Orm.Dapper.Extensions.Expression2Sql
{
    /// <summary>
    /// 模型缓存，第一次用到某个模型时进行属性缓存，下次再用到时将不再重新反射
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ModelCache<T>
    {
        /// <summary>
        /// 属性和真实字段映射
        /// </summary>
        public static Dictionary<string, string> _DicPropertyField { get; private set; }

        /// <summary>
        /// 真实字段和属性映射
        /// </summary>
        public static Dictionary<string, string> _DicFieldProperty { get; private set; }

        /// <summary>
        /// 主键 默认为 ：ID
        /// </summary>
        public static string _PrimaryKey { get { return "ID".ToUpper(); } private set { _PrimaryKey = value; } }

        public static string _TableName { get; }

        public static PropertyInfo[] _Properties { get; private set;}

        public static Type _TType { get; }

        static ModelCache()
        {
            //初始化属性和映射字段
            GetPropertyField();
            //初始化默认 sql
            var type = typeof(T);
            _TType = type;
            //初始化表名称
            _TableName = type.Name;
            var tableAttr = type.GetCustomAttributes(typeof(TableAttribute), false).FirstOrDefault();
            if (tableAttr != null)
            {
                _TableName = ((TableAttribute)tableAttr).Name;
            }
        }


        /// <summary>
        /// 获取属性和字段的映射
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private static void GetPropertyField()
        {
            var type = typeof(T);
            var properties = type.GetProperties();
            _Properties = properties;
            _DicPropertyField = new Dictionary<string, string>();
            _DicFieldProperty = new Dictionary<string, string>();
            foreach (PropertyInfo property in properties)
            {
                //获取 field 别名
                var fieldAttr = property.GetCustomAttributes(typeof(FieldAttribute), false).FirstOrDefault();

                var field = property.Name;
                if (fieldAttr != null)
                {
                    field = ((FieldAttribute)fieldAttr).Name;
                }
                if (!_DicPropertyField.ContainsKey(property.Name.ToUpper()))
                {
                    _DicPropertyField.Add(property.Name.ToUpper(), field.ToUpper());
                }
                if (!_DicFieldProperty.ContainsKey(field.ToUpper()))
                {
                    _DicFieldProperty.Add(field.ToUpper(), property.Name.ToUpper());
                }
                //判断获取主键
                var identityAttr = property.GetCustomAttributes(typeof(PrimaryKeyAttribute), false).FirstOrDefault();
                if (identityAttr != null) _PrimaryKey = _DicPropertyField[property.Name.ToUpper()];

            }
        }
    }
}
