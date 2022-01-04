using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Nitrogen.Orm.Dapper.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public class SqlStorage
    {
        /// <summary>
        /// 查询列
        /// </summary>
        public List<string> SelectFields { get; set; }

        /// <summary>
        /// 查询列SQL片段
        /// </summary>
        public string SelectFieldsStr
        {
            get
            {
                return string.Join(",", this.SelectFields);
            }
        }

        /// <summary>
        /// SQL长度
        /// </summary>
        public int Length
        {
            get
            {
                return this.Sql.Length;
            }
        }

        /// <summary>
        /// SQL子句builder
        /// </summary>
        public StringBuilder Sql { get; set; }

        /// <summary>
        /// 指示是否单表
        /// </summary>
        public bool IsSingleTable { get; set; }

        /// <summary>
        /// 数据库类型
        /// </summary>
        public DatabaseType DatabaseType { get; set; }

        /// <summary>
        /// SQL查询结构 From Where GroupBy Having Select OrderBy
        /// </summary>
        public DQL CurrentDQL { get; set; }

        /// <summary>
        /// 参数
        /// </summary>
        public Dictionary<string, object> DbParams { get; private set; }

        /// <summary>
        /// 参数前缀
        /// </summary>
        private string DbParamPrefix
        {
            get
            {
                switch (this.DatabaseType)
                {
                    case DatabaseType.SQLServer: return "@";
                    case DatabaseType.MySQL: return "?";
                    case DatabaseType.Oracle: return ":";
                    default: return "";
                }
            }
        }

        /// <summary>
        /// 索引
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public char this[int index]
        {
            get
            {
                return this.Sql[index];
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public SqlStorage()
        {
            this.DbParams = new Dictionary<string, object>();
            this.Sql = new StringBuilder();
            this.SelectFields = new List<string>();

        }

        /// <summary>
        /// 清理SQL存储器
        /// </summary>
        public void Clear()
        {
            this.SelectFields.Clear();
            this.Sql.Clear();
            this.DbParams.Clear();
        }

        /// <summary>
        /// +连接操作
        /// </summary>
        /// <param name="sqlStorage"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static SqlStorage operator +(SqlStorage sqlStorage, string sql)
        {
            sqlStorage.Sql.Append(sql);
            return sqlStorage;
        }

        /// <summary>
        /// 添加参数
        /// </summary>
        /// <param name="parameterValue"></param>
        public void AddDbParameter(object parameterValue)
        {
            if (parameterValue == null || parameterValue == DBNull.Value)
            {
                this.Sql.Append(" null");
            }
            else
            {
                string name = this.DbParamPrefix + "param" + this.DbParams.Count;
                this.DbParams.Add(name, parameterValue);
                this.Sql.Append(" " + name);
            }
        }

        /// <summary>
        /// 根据实体表类型获取表名
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string GetTableName(Type type)
        {
            return ((TableAttribute)type.GetCustomAttributes(typeof(TableAttribute), true)[0]).Name;

        }

        /// <summary>
        /// 字符串打印输出
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Sql.ToString();
        }
    }
}
