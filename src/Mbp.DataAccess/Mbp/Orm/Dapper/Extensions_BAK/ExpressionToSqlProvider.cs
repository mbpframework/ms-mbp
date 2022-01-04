using Nitrogen.Orm.Dapper.Extensions.ExpressionCompute;
using System;
using System.Linq.Expressions;

namespace Nitrogen.Orm.Dapper.Extensions
{
    /// <summary>
    /// 表达式解析类
    /// </summary>
    internal static class ExpressionToSqlProvider
    {
        private static IExpressionConverter GetExpression2Sql(Expression expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression", "表达式不能为null");
            }

            if (expression is BinaryExpression)
            {
                return new BinaryExpressionConverter();
            }
            if (expression is ConstantExpression)
            {
                return new ConstantExpressionConverter();
            }
            if (expression is MemberExpression)
            {
                return new MemberExpressionConverter();
            }
            if (expression is MethodCallExpression)
            {
                return new MethodCallExpressionConverter();
            }
            if (expression is NewExpression)
            {
                return new NewExpressionConverter();
            }
            if (expression is UnaryExpression)
            {
                return new UnaryExpressionConverter();
            }
            throw new NotImplementedException("未实现表达式解析");
        }

        public static void Update(Expression expression, SqlStorage sqlStorage)
        {
            GetExpression2Sql(expression).Update(expression, sqlStorage);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="sqlStorage"></param>
        public static void Select(Expression expression, SqlStorage sqlStorage)
        {
            GetExpression2Sql(expression).Select(expression, sqlStorage);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="alias"></param>
        /// <param name="sqlStorage"></param>
        public static void Select(Expression expression, string alias, SqlStorage sqlStorage)
        {
            GetExpression2Sql(expression).Select(expression, alias, sqlStorage);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="sqlStorage"></param>
        public static void Where(Expression expression, SqlStorage sqlStorage)
        {
            GetExpression2Sql(expression).Where(expression, sqlStorage);
        }

        public static void In(Expression expression, SqlStorage sqlStorage)
        {
            GetExpression2Sql(expression).In(expression, sqlStorage);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="sqlStorage"></param>
        public static void OrderBy(Expression expression, SqlStorage sqlStorage)
        {
            GetExpression2Sql(expression).OrderBy(expression, sqlStorage);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="sqlStorage"></param>
        public static void OrderByDescending(Expression expression, SqlStorage sqlStorage)
        {
            GetExpression2Sql(expression).OrderByDescending(expression, sqlStorage);
        }
    }
}
