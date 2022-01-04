using Mbp.Core.User;
using Mbp.Core.User.MultiTenant;
using Mbp.Ddd.Application.Uow;
using Mbp.Ddd.Domain;
using Mbp.Ddd.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;

namespace Mbp.DataAccess.EfCore
{
    /// <summary>
    /// EF Core仓储实现类
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TDbContext"></typeparam>
    public class EfCoreRepository<TEntity, TDbContext, TKey> : INgRepository<TEntity, TDbContext, TKey>, IAsyncEnumerable<TEntity>
          where TEntity : class, IEntity<TKey>
         where TDbContext : DbContext
    {
        private readonly IUnitOfWorkManager _uowManager;
        private IUowDbContextFactory<TDbContext> _dbContextFactory;
        private IQueryable<TEntity> _iqueryable;
        private readonly IOptions<OrmModuleOptions> _options;
        private readonly ICurrentTenant _currentTenant;
        private readonly ICurrentUser _currentUser;

        public virtual DbSet<TEntity> DbSet => DbContext.Set<TEntity>();
        DbContext INgRepository<TEntity, TDbContext, TKey>.DbContext => (DbContext)DbContext;
        protected virtual TDbContext DbContext => _dbContextFactory.GetDbContext();

        public EfCoreRepository(IUnitOfWorkManager uowManager, IOptions<OrmModuleOptions> options, ICurrentTenant currentTenant, ICurrentUser currentUser)
        {
            _uowManager = uowManager;

            _dbContextFactory = new UowDbContextFactory<TDbContext>(_uowManager);

            _iqueryable = DbSet.AsQueryable();
            _options = options;
            _currentTenant = currentTenant;
            _currentUser = currentUser;
        }

        public Type ElementType => GetQueryable().ElementType;

        public Expression Expression => GetQueryable().Expression;

        public IQueryProvider Provider => GetQueryable().Provider;

        public IEnumerator<TEntity> GetEnumerator()
        {
            return GetQueryable().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        protected IQueryable<TEntity> GetQueryable()
        {
            return _iqueryable;
        }

        public IAsyncEnumerator<TEntity> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return DbSet.AsAsyncEnumerable().GetAsyncEnumerator(cancellationToken);
        }

        /// <summary>
        /// 根据主键获取记录对象
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public TEntity Get(TKey Id)
        {
            return DbSet.Where(entity => ((IEntity<TKey>)entity).ID.Equals(Id)).FirstOrDefault();
        }

        /// <summary>
        /// 根据条件获取记录对象
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public TEntity Get(Expression<Func<TEntity, bool>> func)
        {
            if (func != null)
                return DbSet.Where(func).FirstOrDefault();

            return DbSet.FirstOrDefault();
        }

        /// <summary>
        /// 获取仓储中所有对象
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TEntity> GetAll()
        {
            return DbSet;
        }

        /// <summary>
        /// 根据条件获取仓储中的记录列表
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public IEnumerable<TEntity> GetList(Expression<Func<TEntity, bool>> where)
        {
            if (where != null)
                return DbSet.Where(where);
            return DbSet;
        }
    }
}
