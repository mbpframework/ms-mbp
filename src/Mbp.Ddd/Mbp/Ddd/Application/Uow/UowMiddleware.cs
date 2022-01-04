using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Mbp.Ddd.Application.Uow
{
    /// <summary>
    /// 工作单元拦截器
    /// </summary>
    public class UowMiddleware
    {
        private readonly RequestDelegate _next;

        public UowMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync<TDbContext>(HttpContext context, 
            ILogger<UowMiddleware> logger,
            IUnitOfWorkManager unitOfWorkManager)
            where TDbContext : DbContext
        {
            using (var uow = unitOfWorkManager.Create<TDbContext>())
            {
                await _next(context);
            }
        }
    }
}
