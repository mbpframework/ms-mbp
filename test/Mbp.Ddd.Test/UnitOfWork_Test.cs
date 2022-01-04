using Mbp.Core.User.MultiTenant;
using Mbp.Ddd.Application.Uow;
using Mbp.Ddd.Domain;
using Mbp.Ddd.Domain.Aggregate;
using Mbp.Ddd.Domain.Repository;
using Mbp.Test.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Storage;
using Shouldly;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using Xunit;

namespace Mbp.Ddd.Test
{
    public class UnitOfWork_Test : MbpTestBase
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager = null;
        private readonly INgRepository<UserAggregate_Test, DefaultDbContext_Test, Guid> _ngUserRepository = null;

        public UnitOfWork_Test()
        {
            // 因为UnitOfWorkManager共享了UnitOfWork，为了避免并发出现资源争夺问题，取消了线程内共享。
            // 两种方式处理顺序错乱提交问题
            // 1.使用AddScoped<IUnitOfWorkManager, UnitOfWorkManager>()注册
            // 2.使用AsynctLocal来定义IUnitOfWork，我们采用第二种方式，共享工作单元管理器

            ILoggerFactory loggerFactory = LoggerFactory.Create(c =>
             {
                 c.AddConsole();
             });

            services.AddSingleton(loggerFactory.CreateLogger<UnitOfWorkManager>());
            services.AddSingleton(loggerFactory.CreateLogger<UnitOfWork>());

            services.AddSingleton<IUnitOfWorkManager, UnitOfWorkManager>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddDbContext<DefaultDbContext_Test>(options =>
            {
                options.UseMySql("Server=172.18.35.34;Database=Mbp;User=instest;Password=instest;IgnoreCommandTransaction=true;Min Pool Size=5;Max Pool Size=50;",
                 mySqlOptions =>
                 {
                     mySqlOptions
                        .ServerVersion(new ServerVersion(new Version("8.0.18"), ServerType.MySql));
                 });
            });

            // 注册EF CORE泛型仓储
            services.AddScoped(typeof(INgRepository<,,>), typeof(EfCoreRepository_Test<,,>));

            // 模拟注册IConfiguration

            var provider = services.BuildServiceProvider();
            _unitOfWorkManager = provider.GetService<IUnitOfWorkManager>();
            _ngUserRepository = provider.GetService<INgRepository<UserAggregate_Test, DefaultDbContext_Test,Guid>>();
        }

        [Fact]
        public void Create_Test()
        {
            var unitOfWork = _unitOfWorkManager.Create<DefaultDbContext_Test>();

            unitOfWork.ShouldNotBeNull();
        }

        [Fact]
        public async void UnitOfWork_Repository_Test()
        {
            var userget = _ngUserRepository.Get(new Guid("08d8dde7-7eab-4977-866b-9575da3731d9"));

            userget.AGE.ShouldBe(11);

            // 第一个工作单元
            using (var uow = _unitOfWorkManager.Create<DefaultDbContext_Test>())
            {
                var a = _ngUserRepository.DbSet.Add(new UserAggregate_Test()
                {
                    AGE = 11,
                    USER_CODE = "11",
                    USER_NAME = "张三"
                });

                await uow.SaveChangesAsync();
            }

            var users = _ngUserRepository.Where(c => c.AGE > 1).FirstOrDefault();
            users.AGE.ShouldBeGreaterThan(1);
        }
    }

    public class DefaultDbContext_Test : DbContext
    {
        public DefaultDbContext_Test(DbContextOptions<DefaultDbContext_Test> options) : base(options)
        {
        }

        public DbSet<UserAggregate_Test> Users { get; set; }
        public DbSet<BookEntity_Test> Books { get; set; }
    }

    /// <summary>
    /// 示例程序 聚合根
    /// </summary>
    [Table("User")]
    public class UserAggregate_Test : AggregateBase<Guid>, ISoftDelete, IMultiTenant
    {
        public string USER_NAME { get; set; }

        public string USER_CODE { get; set; }

        public int AGE { get; set; }

        public string USER_POSITION { get; set; }

        public List<BookEntity_Test> BOOKS { get; set; }

        public Guid? TENANT_ID { get; set; }

        public int DELETED { get; set; }
    }

    /// <summary>
    /// 示例程序 实体
    /// </summary>
    [Table("Book")]
    public class BookEntity_Test : EntityBase<Guid>, ISoftDelete, IMultiTenant
    {
        public string BOOK_NAME { get; set; }

        public string BOOK_CODE { get; set; }

        public decimal BOOK_PRICE { get; set; }

        public Guid? TENANT_ID { get; set; }

        public int DELETED { get; set; }
    }

    public class EfCoreRepository_Test<TEntity, TDbContext, TKey> : INgRepository<TEntity, TDbContext, TKey>, IAsyncEnumerable<TEntity>
          where TEntity : class, IEntity<TKey>
         where TDbContext : DbContext
    {
        private readonly IUnitOfWorkManager _uowManager;
        private IUowDbContextFactory<TDbContext> _dbContextFactory;
        private IQueryable<TEntity> _iqueryable;

        public virtual DbSet<TEntity> DbSet => DbContext.Set<TEntity>();
        DbContext INgRepository<TEntity, TDbContext, TKey>.DbContext => (DbContext)DbContext;
        protected virtual TDbContext DbContext => _dbContextFactory.GetDbContext();

        public EfCoreRepository_Test(IUnitOfWorkManager uowManager)
        {
            _uowManager = uowManager;

            _dbContextFactory = new UowDbContextFactory<TDbContext>(_uowManager);

            _iqueryable = DbSet.AsQueryable();
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

        public TEntity Get(TKey Id, string tableName = null)
        {
            throw new NotImplementedException();
        }

        public TEntity Get(Expression<Func<TEntity, bool>> func, string tableName = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TEntity> GetAll(string tableName = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TEntity> GetList(Expression<Func<TEntity, bool>> where, string tableName = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TEntity> GetListOrderBy(Expression<Func<TEntity, bool>> where = null, Expression<Func<TEntity, object>> order = null, string tableName = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TEntity> GetListOrderByDescending(Expression<Func<TEntity, bool>> where = null, Expression<Func<TEntity, object>> order = null, string tableName = null)
        {
            throw new NotImplementedException();
        }

        public (IEnumerable<TEntity>, int, int) PagingQueryTradition(Expression<Func<TEntity, object>> selectColumns, Expression<Func<TEntity, bool>> whereSql, int pageIndex, int pageSize, Expression<Func<TEntity, object>> orderBy, string tableName = null)
        {
            throw new NotImplementedException();
        }

        public int Update(TEntity entity, string tableName = null)
        {
            throw new NotImplementedException();
        }

        public int Update(Expression<Func<TEntity>> expression, Expression<Func<TEntity, bool>> where, string tableName = null)
        {
            throw new NotImplementedException();
        }

        public int Insert(TEntity entity, string tableName = null)
        {
            throw new NotImplementedException();
        }

        public int Delete(TEntity entity, string tableName = null)
        {
            throw new NotImplementedException();
        }
    }
}
