using Microsoft.EntityFrameworkCore;
using Nitrogen.Ddd.Domain;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Nitrogen.Ddd.Domain.Repository
{
    /// <summary>
    /// 可根据指定的表名来替换默认的类型映射表名
    /// </summary>
    public interface IShardingRepository<TEntity, TKey> : INgRepository
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
    }
}
