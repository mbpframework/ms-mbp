using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Mbp.Ddd.Application.Uow
{
    /// <summary>
    /// 工作单元管理类
    /// </summary>
    public class UnitOfWorkManager : IUnitOfWorkManager
    {
        //private IUnitOfWork currentUow;
        private AsyncLocal<IUnitOfWork> currentUow;
        private bool _isDisposed = false;

        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<UnitOfWorkManager> _logger;

        public UnitOfWorkManager(
            IServiceProvider serviceProvider, ILogger<UnitOfWorkManager> logger)
        {
            _serviceProvider = serviceProvider;
            currentUow = new AsyncLocal<IUnitOfWork>();
            _logger = logger;
        }

        /// <summary>
        /// 创建工作单元,工作单元默认就是事务性
        /// </summary>
        /// <returns></returns>
        public IUnitOfWork Create<TDbContext>(bool requiresNew = true)
            where TDbContext : DbContext
        {
            if (requiresNew)
                return Create<TDbContext>(new UnitOfWorkOptions());
            return currentUow.Value ?? Create<TDbContext>(new UnitOfWorkOptions());
        }

        public IUnitOfWork Create<TDbContext>(UnitOfWorkOptions options)
            where TDbContext : DbContext
        {
            CreateNewUnitOfWork(options);

            // 使用uow解析出Dbcontext，使得同一个uow对应一个DbContext，并且都是同一个请求的。
            var wantedDbContext = (TDbContext)currentUow.Value.ServiceProvider.GetService(typeof(TDbContext));

            // 解析之前必须使用EF CORE进行注册。
            if (wantedDbContext == null)
                throw new NullReferenceException("Cannot get DbContext.Please check add ef services correctly");

            // UnitofWork上开启事务
            AddDbTransactionFeatureToUow(currentUow.Value, wantedDbContext);

            return currentUow.Value;
        }

        private void AddDbTransactionFeatureToUow<TDbContext>(IUnitOfWork uow, TDbContext dbContext)
             where TDbContext : DbContext
        {
            string key = $"EFCore - {dbContext.ContextId.InstanceId.ToString()}";

            var efFeature = (EfCoreTransactionFeature)uow.GetOrAddTransactionFeature(key, new EfCoreTransactionFeature(dbContext));

            if (IsFeatureNeedOpenTransaction(efFeature))
            {
                var dbcontextTransaction = uow.UnitOfWorkOptions.IsolationLevel.HasValue ?
                                            dbContext.Database.BeginTransaction(uow.UnitOfWorkOptions.IsolationLevel.Value) :
                                            dbContext.Database.BeginTransaction();

                efFeature.SetTransaction(dbcontextTransaction);
            }
        }

        private bool IsFeatureNeedOpenTransaction(EfCoreTransactionFeature efFeature)
        {
            return !efFeature.IsOpenTransaction;
        }

        public IUnitOfWork GetCurrentUnitOfWork()
        {
            return currentUow.Value ?? CreateNewUnitOfWork(new UnitOfWorkOptions());
        }

        // Create a new unitofwork 
        private IUnitOfWork CreateNewUnitOfWork(UnitOfWorkOptions options)
        {
            IUnitOfWork result;

            var uowScope = _serviceProvider.CreateScope();

            try
            {
                result = uowScope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                if (options != null)
                    result.SetOptions(options);

                result.DisposeHandler += (sender, args) =>
                {
                    uowScope.Dispose();
                    currentUow.Value = null;
                };
            }
            catch (Exception)
            {
                uowScope.Dispose();
                throw;
            }

            currentUow.Value = result;

            return result;
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                _logger.LogWarning(new Exception("this manager is already disposed"), "工作单元管理器析构异常！");
            }

            _isDisposed = true;

            currentUow.Value?.Dispose();
        }
    }
}
