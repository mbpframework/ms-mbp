using System.Linq.Expressions;

namespace Nitrogen.Orm.Dapper.Extensions.ExpressionCompute
{
    /// <summary>
    /// 表示具有一元运算符的表达式转换器
    /// </summary>
    public class UnaryExpressionConverter : BaseExpressionConverter<UnaryExpression>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="sqlStorage"></param>
        /// <returns></returns>
        public override SqlStorage Select(UnaryExpression expression, SqlStorage sqlStorage)
        {
            ExpressionToSqlProvider.Select(expression.Operand, sqlStorage);
            return sqlStorage;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="sqlStorage"></param>
        /// <returns></returns>
        public override SqlStorage Where(UnaryExpression expression, SqlStorage sqlStorage)
        {
            ExpressionToSqlProvider.Where(expression.Operand, sqlStorage);
            return sqlStorage;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="sqlStorage"></param>
        /// <returns></returns>
        public override SqlStorage OrderBy(UnaryExpression expression, SqlStorage sqlStorage)
        {
            ExpressionToSqlProvider.OrderBy(expression.Operand, sqlStorage);
            return sqlStorage;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="sqlStorage"></param>
        /// <returns></returns>
        public override SqlStorage OrderByDescending(UnaryExpression expression, SqlStorage sqlStorage)
        {
            ExpressionToSqlProvider.OrderByDescending(expression.Operand, sqlStorage);
            return sqlStorage;
        }
    }
}
