using Microsoft.EntityFrameworkCore;
using WuhanIns.Nitrogen.Ddd.Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace WuhanIns.Nitrogen.Ddd.Domain.Repository
{
    /// <summary>
    /// Dapper 仓储
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TDbContext"></typeparam>
    public interface IDapperRepository<TEntity, TDbContext,TKey> : IReadOnlyRepository<TEntity, TKey>, IWriteableRepository<TEntity, TKey>
        where TEntity : class, IEntity<TKey>
        where TDbContext : DbContext
    {
        /// <summary>
        /// 连接对象，提供Dapper原有操作的基础 屏蔽：SQL跟踪 需要调整源码
        /// </summary>
        //IDbConnection DbConnection { get; }
    }
}
