using Mbp.Ddd.Application.UI;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Mbp.Ddd.Domain.Repository
{
    /// <summary>
    /// 只读类仓储接口 代办 异步方法定义
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public interface IReadOnlyRepository<TEntity, TKey> : IMbpRepository
        where TEntity : class, IEntity<TKey>
    {
        /// <summary>
        /// 根据主键获取一条记录
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        TEntity Get(TKey Id, string tableName = null);

        /// <summary>
        /// 根据条件获取一条记录
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        TEntity Get(Expression<Func<TEntity, bool>> func, string tableName = null);

        /// <summary>
        /// 获取全部
        /// </summary>
        /// <returns></returns>
        IEnumerable<TEntity> GetAll(string tableName = null);

        /// <summary>
        /// 根据条件获取数据列表
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        IEnumerable<TEntity> GetList(Expression<Func<TEntity, bool>> where, string tableName = null);

        /// <summary>
        /// 根据条件获取数据列表，并升序返回
        /// </summary>
        /// <param name="where"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        IEnumerable<TEntity> GetListOrderBy(Expression<Func<TEntity, bool>> where = null, Expression<Func<TEntity, object>> order = null, string tableName = null);

        /// <summary>
        /// 根据条件获取数据列表，并降序返回
        /// </summary>
        /// <param name="where"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        IEnumerable<TEntity> GetListOrderByDescending(Expression<Func<TEntity, bool>> where = null, Expression<Func<TEntity, object>> order = null, string tableName = null);

        /// <summary>
        /// 常规分页
        /// </summary>
        /// <typeparam name="TDTO">返回的结果类型，需要是class</typeparam>
        /// <param name="selectColumns">返回列</param>
        /// <param name="whereSql">条件语句</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="orderBy"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        (IEnumerable<TEntity>, int, int) PagingQueryTradition(Expression<Func<TEntity, object>> selectColumns, Expression<Func<TEntity, bool>> whereSql, int pageIndex, int pageSize, Expression<Func<TEntity, object>> orderBy, string tableName = null);
    }
}
