using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Expressions;

namespace Mbp.Ddd.Domain.Repository
{
    /// <summary>
    /// 可写数据的接口定义，使用dapper实现
    /// </summary>
    public interface IWriteableRepository<TEntity, TKey> : IMbpRepository
        where TEntity : class, IEntity<TKey>
    {
        /// <summary>
        /// 更新记录，以ID为主键作为更新依据
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        int Update(TEntity entity, string tableName = null);

        /// <summary>
        /// 更新记录
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="expression"></param>
        /// <param name="where">必填，条件</param>
        /// <returns></returns>
        int Update(Expression<Func<TEntity>> expression, Expression<Func<TEntity, bool>> where, string tableName = null);

        /// <summary>
        /// 新增记录
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        int Insert(TEntity entity, string tableName = null);

        /// <summary>
        /// 删除记录
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        int Delete(TEntity entity, string tableName = null);
    }
}
