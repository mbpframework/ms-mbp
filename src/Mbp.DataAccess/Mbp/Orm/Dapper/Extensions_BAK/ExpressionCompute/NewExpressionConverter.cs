using System.Linq.Expressions;
using System.Reflection;

namespace Nitrogen.Orm.Dapper.Extensions.ExpressionCompute
{
    /// <summary>
    /// 表示一个构造函数调用。
    /// </summary>
    public class NewExpressionConverter : BaseExpressionConverter<NewExpression>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="sqlStorage"></param>
        /// <returns></returns>
        public override SqlStorage Update(NewExpression expression, SqlStorage sqlStorage)
        {
            for (int i = 0; i < expression.Members.Count; i++)
            {
                MemberInfo m = expression.Members[i];
                ConstantExpression c = expression.Arguments[i] as ConstantExpression;
                sqlStorage += m.Name + " =";
                sqlStorage.AddDbParameter(c.Value);
                sqlStorage += ",";
            }
            if (sqlStorage[sqlStorage.Length - 1] == ',')
            {
                sqlStorage.Sql.Remove(sqlStorage.Length - 1, 1);
            }
            return sqlStorage;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="sqlStorage"></param>
        /// <returns></returns>
        public override SqlStorage Select(NewExpression expression, SqlStorage sqlStorage)
        {
            int memberIndex = 0;
            foreach (Expression item in expression.Arguments)
            {
                if (item.ToString().Contains(expression.Members[memberIndex].Name))
                {
                    ExpressionToSqlProvider.Select(item, sqlStorage);
                }
                else
                {
                    ExpressionToSqlProvider.Select(item, expression.Members[memberIndex].Name, sqlStorage);
                }

                memberIndex++;
            }
            return sqlStorage;
        }

        public override SqlStorage Where(NewExpression expression, SqlStorage sqlStorage)
        {
            sqlStorage += "'" + (expression.Arguments[0] as ConstantExpression).Value + "'";
            return sqlStorage;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="sqlStorage"></param>
        /// <returns></returns>
        public override SqlStorage OrderBy(NewExpression expression, SqlStorage sqlStorage)
        {
            foreach (Expression item in expression.Arguments)
            {
                ExpressionToSqlProvider.OrderBy(item, sqlStorage);
                sqlStorage.Sql.Append(",");
            }
            sqlStorage.Sql.Remove(sqlStorage.Sql.Length - 1, 1);
            return sqlStorage;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="sqlStorage"></param>
        /// <returns></returns>
        public override SqlStorage OrderByDescending(NewExpression expression, SqlStorage sqlStorage)
        {
            foreach (Expression item in expression.Arguments)
            {
                ExpressionToSqlProvider.OrderByDescending(item, sqlStorage);
                sqlStorage.Sql.Append(",");
            }
            sqlStorage.Sql.Remove(sqlStorage.Sql.Length - 1, 1);
            return sqlStorage;
        }
    }
}
