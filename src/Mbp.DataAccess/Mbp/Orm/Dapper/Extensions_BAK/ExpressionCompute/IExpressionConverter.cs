using System.Linq.Expressions;

namespace Nitrogen.Orm.Dapper.Extensions.ExpressionCompute
{
    /// <summary>
    /// 表达式树转换器接口
    /// </summary>
    public interface IExpressionConverter
    {
        SqlStorage Update(Expression expression, SqlStorage sqlStorage);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="sqlStorage"></param>
        /// <returns></returns>
        SqlStorage Select(Expression expression, SqlStorage sqlStorage);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="alias"></param>
        /// <param name="sqlStorage"></param>
        /// <returns></returns>
        SqlStorage Select(Expression expression, string alias, SqlStorage sqlStorage);

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="sqlStorage"></param>
        /// <returns></returns>
        SqlStorage Where(Expression expression, SqlStorage sqlStorage);

        SqlStorage In(Expression expression, SqlStorage sqlStorage);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="sqlStorage"></param>
        /// <returns></returns>
        SqlStorage OrderBy(Expression expression, SqlStorage sqlStorage);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="sqlStorage"></param>
        /// <returns></returns>
        SqlStorage OrderByDescending(Expression expression, SqlStorage sqlStorage);
    }
}
