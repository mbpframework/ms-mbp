using Nitrogen.Ddd.Domain;
using Nitrogen.Ddd.Extensions;
using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Nitrogen.Orm.Dapper.Extensions.ExpressionCompute
{
    /// <summary>
    /// 表示访问字段或属性。
    /// </summary>
    public class MemberExpressionConverter : BaseExpressionConverter<MemberExpression>
    {
        private static readonly string s_isHasConstantPatter = "value((.*?)).(.*?)";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="sqlStorage"></param>
        /// <returns></returns>
        public override SqlStorage Select(MemberExpression expression, SqlStorage sqlStorage)
        {
            string tableName = sqlStorage.GetTableName(expression.Expression.Type);
            if (!string.IsNullOrWhiteSpace(tableName))
            {
                tableName += ".";
            }
            sqlStorage.SelectFields.Add(tableName + expression.Member.Name);
            return sqlStorage;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="alias"></param>
        /// <param name="sqlStorage"></param>
        /// <returns></returns>
        public override SqlStorage Select(MemberExpression expression, string alias, SqlStorage sqlStorage)
        {
            string tableName = sqlStorage.GetTableName(expression.Expression.Type);
            if (!string.IsNullOrWhiteSpace(tableName))
            {
                tableName += ".";
            }

            sqlStorage.SelectFields.Add(tableName + "[" + expression.Member.Name + "]" + " AS " + alias);
            return sqlStorage;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="sqlStorage"></param>
        /// <returns></returns>
        public override SqlStorage Where(MemberExpression expression, SqlStorage sqlStorage)
        {
            //访问表达式分析是否需要取值还是简单的成员访问表达式 value(Dapper.Test.Program+<>c__DisplayClass0_0).user.Userguid
            if (Regex.IsMatch(expression.ToString(), s_isHasConstantPatter))
            {
                object value = Expression.Lambda(expression).Compile().DynamicInvoke();

                sqlStorage.AddDbParameter(value);
            }
            else if (expression.Expression != null && typeof(IEntity).IsAssignableFrom(expression.Expression.Type))
            {
                //实体字段访问
                string tableName = sqlStorage.GetTableName(expression.Expression.Type);
                if (!string.IsNullOrWhiteSpace(tableName))
                {
                    tableName += ".";
                }
                sqlStorage += " " + tableName + expression.Member.Name;
            }
            else if (expression.ToString() == "String.Empty")
            {
                //过滤引用类型String string.Empty,其他自定义的引用类型用null禁止使用自己实现的Empty成员
                sqlStorage.AddDbParameter("");
            }
            else if (expression.Expression is ConstantExpression)
            {
                //where 条件 引用变量
                var @object = ((ConstantExpression)(expression.Expression)).Value;
                var value = new object();
                if (expression.Member.MemberType == MemberTypes.Field)
                {
                    value = ((FieldInfo)expression.Member).GetValue(@object);
                }
                else if (expression.Member.MemberType == MemberTypes.Property)
                {
                    value = ((PropertyInfo)expression.Member).GetValue(@object);
                }
                sqlStorage.AddDbParameter(value);
            }
            else
            {
                //过滤值类型的成员访问
                Type t = expression.Member.DeclaringType;
                sqlStorage.AddDbParameter(t.GetValueTypeDefaultValue());
            }
            return sqlStorage;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="sqlStorage"></param>
        /// <returns></returns>
        public override SqlStorage OrderBy(MemberExpression expression, SqlStorage sqlStorage)
        {
            sqlStorage += sqlStorage.GetTableName(expression.Expression.Type) + "." + expression.Member.Name + " ASC";
            return sqlStorage;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="sqlStorage"></param>
        /// <returns></returns>
        public override SqlStorage OrderByDescending(MemberExpression expression, SqlStorage sqlStorage)
        {
            sqlStorage += sqlStorage.GetTableName(expression.Expression.Type) + "." + expression.Member.Name + " DESC";
            return sqlStorage;
        }
    }
}
