using System.Linq.Expressions;

namespace Nitrogen.Orm.Dapper.Extensions.ExpressionCompute
{
    /// <summary>
    /// 表示具有常量值的表达式
    /// </summary>
    public class ConstantExpressionConverter : BaseExpressionConverter<ConstantExpression>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="sqlStorage"></param>
        /// <returns></returns>
        public override SqlStorage Where(ConstantExpression expression, SqlStorage sqlStorage)
        {
            sqlStorage.AddDbParameter(expression.Value);
            return sqlStorage;
        }

    }
}
