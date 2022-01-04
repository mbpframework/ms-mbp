using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Text;

namespace Mbp.Ddd.Mbp.Ddd.Domain.Extensions
{
    /// <summary>
    /// 实体类型扩展
    /// </summary>
    public static class IEntityExtensions
    {
        /// <summary>
        /// 获取实体映射的表名
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetTableName(this Type type)
        {
            var attrTable = type.GetCustomAttributes(typeof(TableAttribute), true);
            if (attrTable != null && attrTable.Length > 0)
            {
                return ((TableAttribute)attrTable[0]).Name;
            }

            return type.Name;
        }

        /// <summary>
        /// 得到实体的全字段查询列
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        public static string GetTableColumns(this Type type, string tableName = null)
        {
            StringBuilder columnsBuilder = new StringBuilder();
            var props = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (var p in props)
            {
                if (string.IsNullOrEmpty(tableName))
                    columnsBuilder.Append(string.Concat(type.GetTableName(), ".", p.Name));
                else
                    columnsBuilder.Append(string.Concat(tableName, ".", p.Name));
                columnsBuilder.Append(",");
            }

            columnsBuilder.Remove(columnsBuilder.Length - 1, 1);

            return columnsBuilder.ToString();
        }

        public static object GetColumnValue(this Type type, string columnName,object obj)
        {
            var props = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (var p in props)
            {
                if (p.Name == columnName)
                {
                    return p.GetValue(obj);
                }
            }

            return null;
        }
    }
}
