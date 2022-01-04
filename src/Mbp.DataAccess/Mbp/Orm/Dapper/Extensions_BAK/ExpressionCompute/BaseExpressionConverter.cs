using System;
using System.Linq.Expressions;

namespace Nitrogen.Orm.Dapper.Extensions.ExpressionCompute
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TExpression"></typeparam>
    public class BaseExpressionConverter<TExpression> : IExpressionConverter where TExpression : Expression
    {
        public virtual SqlStorage Update(TExpression expression, SqlStorage sqlStorage)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="sqlStorage"></param>
        /// <returns></returns>
        public virtual SqlStorage Select(TExpression expression, SqlStorage sqlStorage)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="alias"></param>
        /// <param name="sqlStorage"></param>
        /// <returns></returns>
        public virtual SqlStorage Select(TExpression expression, string alias, SqlStorage sqlStorage)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="sqlStorage"></param>
        /// <returns></returns>
        public virtual SqlStorage Where(TExpression expression, SqlStorage sqlStorage)
        {
            throw new NotImplementedException();
        }

        public virtual SqlStorage In(TExpression expression, SqlStorage sqlStorage)
        {
            throw new NotImplementedException();
        }


        public SqlStorage Update(Expression expression, SqlStorage sqlStorage)
        {
            return Update((TExpression)expression, sqlStorage);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="sqlStorage"></param>
        /// <returns></returns>
        public SqlStorage Select(Expression expression, SqlStorage sqlStorage)
        {
            return Select((TExpression)expression, sqlStorage);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="alias"></param>
        /// <param name="sqlStorage"></param>
        /// <returns></returns>
        public SqlStorage Select(Expression expression, string alias, SqlStorage sqlStorage)
        {
            return Select((TExpression)expression, alias, sqlStorage);
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="sqlStorage"></param>
        /// <returns></returns>
        public SqlStorage Where(Expression expression, SqlStorage sqlStorage)
        {
            return Where((TExpression)expression, sqlStorage);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="sqlStorage"></param>
        /// <returns></returns>
        public virtual SqlStorage OrderBy(TExpression expression, SqlStorage sqlStorage)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="sqlStorage"></param>
        /// <returns></returns>
        public virtual SqlStorage OrderByDescending(TExpression expression, SqlStorage sqlStorage)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="sqlStorage"></param>
        /// <returns></returns>
        public SqlStorage OrderBy(Expression expression, SqlStorage sqlStorage)
        {
            return OrderBy((TExpression)expression, sqlStorage);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="sqlStorage"></param>
        /// <returns></returns>
        public SqlStorage OrderByDescending(Expression expression, SqlStorage sqlStorage)
        {
            return OrderByDescending((TExpression)expression, sqlStorage);
        }

        public SqlStorage In(Expression expression, SqlStorage sqlStorage)
        {
            return In((TExpression)expression, sqlStorage);
        }
    }
}
