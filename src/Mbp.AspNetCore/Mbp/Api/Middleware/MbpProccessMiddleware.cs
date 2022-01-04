using Mbp.Core;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mbp.AspNetCore.Api.Middleware
{
    /// <summary>
    /// Mbp进行上下文中间件，对上下文进行初始化工作
    /// </summary>
    internal class MbpProccessMiddleware
    {
        private readonly RequestDelegate _next;

        public MbpProccessMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        // 所有的请求 都会初始化一个跟踪堆栈信息。
        public async Task InvokeAsync(HttpContext context, IMbpContextAccessor MbpContextAccessor)
        {
            // 为每个请求上下文设置唯一标识并初始化数据项,初始化跟踪堆栈
            MbpContextAccessor.MbpContext = new MbpContext()
            {
                TraceIdentifier = Guid.NewGuid().ToString()
            };

            // 执行请求
            await _next(context);
        }
    }
}
