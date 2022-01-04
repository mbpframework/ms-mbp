using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Mbp.Core.User.MultiTenant
{
    /// <summary>
    /// 多租户中间件，负责多租户解析器运行并初始化多租户信息
    /// </summary>
    public class MbpMultiTenantMiddleware
    {
        private readonly RequestDelegate _next;

        public MbpMultiTenantMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ILogger<MbpMultiTenantMiddleware> logger)
        {
            await _next(context);
        }
    }
}
