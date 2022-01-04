using Microsoft.EntityFrameworkCore;
using Mbp.Ddd.Application.Uow;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mbp.Ddd.Application.Uow
{
    public class UowDbContextFactory<TDbContext> : IUowDbContextFactory<TDbContext>
         where TDbContext : DbContext
    {
        private readonly IUnitOfWorkManager _uowManager;

        public UowDbContextFactory(IUnitOfWorkManager uowManager)
        {
            _uowManager = uowManager;
        }

        /// <summary>
        /// 获取上下文DbContext
        /// </summary>
        /// <returns></returns>
        public TDbContext GetDbContext()
        {
            // uowManager是全局的，唯一的，uow是请求级别的。
            var currentUow = _uowManager.GetCurrentUnitOfWork();

            if (currentUow == null)
                throw new NullReferenceException("Cannot get a unit of work,Please check create root unit of work correctly");

            // 使用uow解析出Dbcontext，使得同一个uow对应一个DbContext，并且都是同一个请求的。
            var wantedDbContext = (TDbContext)currentUow.ServiceProvider.GetService(typeof(TDbContext));

            // 解析之前必须使用EF CORE进行注册。
            if (wantedDbContext == null)
                throw new NullReferenceException("Cannot get DbContext.Please check add ef services correctly");

            return wantedDbContext;
        }
    }
}
