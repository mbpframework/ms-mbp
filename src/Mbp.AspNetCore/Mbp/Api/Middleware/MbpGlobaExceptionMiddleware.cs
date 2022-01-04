using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mbp.Utils;
using Mbp.Core;

namespace Mbp.AspNetCore.Api.Middleware
{
    /// <summary>
    /// 应用层全局异常处理中间件,中断路由，成为新的终结点
    /// </summary>
    internal class MbpGlobaExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public MbpGlobaExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ILogger<MbpGlobaExceptionMiddleware> logger)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }

                if (ex.GetType() == typeof(ConcurrentException))
                {
                    // 发生冲突时候,牺牲后者.不做具体数据合并操作.提示当前用户数据已经发生修改,需要重试.
                    logger.LogError("并发冲突:" + ex.Message);

                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(new { state = 500, message = "提交并发冲突", version = "1", content = new List<object>() }, new JsonSerializerSettings()
                    {
                        DateFormatString = "yyyy-MM-dd HH:mm:ss"
                    }));
                }
                else if (ex is PromptingException)
                {
                    // 其他异常
                    logger.LogWarning($"请求[{context.Request.Path}]发生异常:" + $"{ex.Message}\n{ex.StackTrace}");

                    context.Response.ContentType = "application/json";

                    // 捕获子请求的异常不记录当前堆栈中
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(new { state = 500, message = $"{ex.Message}", version = "1", content = new List<object>() }, new JsonSerializerSettings()
                    {
                        DateFormatString = "yyyy-MM-dd HH:mm:ss"
                    }));
                }
                else
                {
                    // 其他异常
                    logger.LogError($"请求[{context.Request.Path}]发生异常:" + $"{ex.Message}\n{ex.StackTrace}");

                    context.Response.ContentType = "application/json";

                    // 捕获子请求的异常不记录当前堆栈中

                    await context.Response.WriteAsync(JsonConvert.SerializeObject(new { state = 500, message = $"{ex.Message}", version = "1", content = new List<object>() }, new JsonSerializerSettings()
                    {
                        DateFormatString = "yyyy-MM-dd HH:mm:ss"
                    }));
                }
            }
        }
    }
}
