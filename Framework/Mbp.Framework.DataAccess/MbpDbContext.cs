using Mbp.Core.User.MultiTenant;
using Mbp.Ddd.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Mbp.Framework.DataAccess
{
    /// <summary>
    /// 底层DbContext，可以定制数据库初始化行为
    /// </summary>
    /// <typeparam name="TDbContext"></typeparam>
    public class MbpDbContext<TDbContext> : DbContext
        where TDbContext : DbContext
    {
        /// <summary>
        /// 当前租户Id
        /// </summary>
        protected virtual Guid? CurrentTenantId => CurrentTenant?.TenantId;

        /// <summary>
        /// 当前租户对象
        /// </summary>
        public ICurrentTenant CurrentTenant { get; private set; }

        /// <summary>
        /// 数据过滤
        /// </summary>

        // 配置全局筛选器
        private static readonly MethodInfo ConfigureBasePropertiesMethodInfo
            = typeof(MbpDbContext<TDbContext>)
                .GetMethod(
                    nameof(ConfigureBaseProperties),
                    BindingFlags.Instance | BindingFlags.NonPublic
                );

        public MbpDbContext(DbContextOptions<TDbContext> options) : base(options)
        {
            CurrentTenant = base.Database.GetService<ICurrentTenant>();
        }

        // 创建模型时候触发
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                ConfigureBasePropertiesMethodInfo
                    .MakeGenericMethod(entityType.ClrType)
                    .Invoke(this, new object[] { modelBuilder, entityType });
            }
        }

        // 配置全局筛选器
        protected virtual void ConfigureBaseProperties<TEntity>(ModelBuilder modelBuilder, IMutableEntityType mutableEntityType)
            where TEntity : class
        {
            if (mutableEntityType.IsOwned())
            {
                return;
            }

            if (!typeof(IEntity).IsAssignableFrom(typeof(TEntity)))
            {
                return;
            }

            //modelBuilder.Entity<TEntity>().ConfigureByConvention();
            modelBuilder.Entity<TEntity>();

            ConfigureGlobalFilters<TEntity>(modelBuilder, mutableEntityType);
        }

        // 配置全局筛选器
        protected virtual void ConfigureGlobalFilters<TEntity>(ModelBuilder modelBuilder, IMutableEntityType mutableEntityType)
            where TEntity : class
        {
            if (mutableEntityType.BaseType == null && ShouldFilterEntity<TEntity>(mutableEntityType))
            {
                var filterExpression = CreateFilterExpression<TEntity>();
                if (filterExpression != null)
                {
                    modelBuilder.Entity<TEntity>().HasQueryFilter(filterExpression);
                }
            }
        }

        // 重写保存方法
        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            // 代办:审计支持 目前暂时不做审计支持
            ApplyMbpConcepts();

            var result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);

            return result;
        }

        // 重写保存方法
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            // 代办:审计支持 目前暂时不做审计支持
            ApplyMbpConcepts();

            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // 代办:审计支持 目前暂时不做审计支持
            ApplyMbpConcepts();

            return base.SaveChangesAsync(cancellationToken);
        }

        protected virtual void ApplyMbpConcepts()
        {
            foreach (var entry in ChangeTracker.Entries().ToList())
            {
                ApplyAbpConcepts(entry);
            }
        }

        protected virtual void ApplyAbpConcepts(EntityEntry entry)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    // 代办 审计
                    break;
                case EntityState.Modified:
                    // 代办 审计
                    break;
                case EntityState.Deleted:
                    ApplyAbpConceptsForDeletedEntity(entry);
                    break;
            }

            // 代办 保存扩展实体信息

            // 代办 添加领域事件
        }

        protected virtual void ApplyAbpConceptsForDeletedEntity(EntityEntry entry)
        {
            TryCancelDeletionForSoftDelete(entry);
        }

        protected virtual bool TryCancelDeletionForSoftDelete(EntityEntry entry)
        {
            if (!(entry.Entity is ISoftDelete))
            {
                return false;
            }

            entry.Reload();
            entry.State = EntityState.Modified;
            entry.Entity.As<ISoftDelete>().DELETED = 1;
            return true;
        }

        // 创建筛选器表达式
        protected virtual Expression<Func<TEntity, bool>> CreateFilterExpression<TEntity>()
           where TEntity : class
        {
            Expression<Func<TEntity, bool>> expression = null;

            if (typeof(ISoftDelete).IsAssignableFrom(typeof(TEntity)))
            {
                expression = e => EF.Property<int>(e, "DELETED") != 1;
            }

            if (typeof(IMultiTenant).IsAssignableFrom(typeof(TEntity)))
            {
                Expression<Func<TEntity, bool>> multiTenantFilter = e => EF.Property<Guid>(e, "TENANT_ID") == CurrentTenantId;
                expression = expression == null ? multiTenantFilter : CombineExpressions(expression, multiTenantFilter);
            }

            return expression;
        }

        // 连接2个表达式
        protected virtual Expression<Func<T, bool>> CombineExpressions<T>(Expression<Func<T, bool>> expression1, Expression<Func<T, bool>> expression2)
        {
            var parameter = Expression.Parameter(typeof(T));

            var leftVisitor = new ReplaceExpressionVisitor(expression1.Parameters[0], parameter);
            var left = leftVisitor.Visit(expression1.Body);

            var rightVisitor = new ReplaceExpressionVisitor(expression2.Parameters[0], parameter);
            var right = rightVisitor.Visit(expression2.Body);

            return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(left, right), parameter);
        }

        /// <summary>
        /// 判断是否需要查询筛选操作
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entityType"></param>
        /// <returns></returns>
        protected virtual bool ShouldFilterEntity<TEntity>(IMutableEntityType entityType) where TEntity : class
        {
            if (typeof(IMultiTenant).IsAssignableFrom(typeof(TEntity)))
            {
                return true;
            }

            if (typeof(ISoftDelete).IsAssignableFrom(typeof(TEntity)))
            {
                return true;
            }

            return false;
        }

        class ReplaceExpressionVisitor : ExpressionVisitor
        {
            private readonly Expression _oldValue;
            private readonly Expression _newValue;

            public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
            {
                _oldValue = oldValue;
                _newValue = newValue;
            }

            public override Expression Visit(Expression node)
            {
                if (node == _oldValue)
                {
                    return _newValue;
                }

                return base.Visit(node);
            }
        }
    }
}
