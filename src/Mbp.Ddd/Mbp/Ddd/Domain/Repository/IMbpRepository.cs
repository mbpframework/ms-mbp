using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Mbp.Ddd.Domain.Repository
{
    /// <summary>
    /// 指示仓储实现类
    /// </summary>
    public interface IMbpRepository
    {
    }

    /// <summary>
    /// 仓储接口，含读写
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TDbContext"></typeparam>
    public interface INgRepository<TEntity, TDbContext, TKey> : IQueryable<TEntity>
        where TEntity : class, IEntity<TKey>
        where TDbContext : DbContext
    {
        // EF CORE的默认使用仓储，因为EF CORE本身就集中了仓储和工作单元的实现，故不在考虑进行二次封装
        // 风险：更换主场景开发的ORM。

        /// <summary>
        /// EF CORE 上下文
        /// </summary>
        DbContext DbContext { get; }

        /// <summary>
        /// 仓储数据集
        /// </summary>
        DbSet<TEntity> DbSet { get; }

        /// <summary>
        /// 根据主键获取一条记录
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        TEntity Get(TKey Id);

        /// <summary>
        /// 根据条件获取一条记录
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        TEntity Get(Expression<Func<TEntity, bool>> func);

        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <returns></returns>
        IEnumerable<TEntity> GetAll();

        /// <summary>
        /// 根据条件获取数据列表
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        IEnumerable<TEntity> GetList(Expression<Func<TEntity, bool>> where);
    }
}
