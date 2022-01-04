using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mbp.AspNetCore.Permission
{
    /// <summary>
    /// 权限过滤器
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class MbpPermissionAttribute : Attribute, IAsyncAuthorizationFilter
    {
        /// <summary>
        /// 权限要求
        /// </summary>
        /// <param name="actionCode">权限编码</param>
        public MbpPermissionAttribute(params string[] actionCodes)
        {
            ActionCodes = actionCodes;
        }

        public string[] ActionCodes { get; private set; }

        /// <summary>
        /// 授权验证
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            // 获取授权服务
            var authorizationService = context.HttpContext.RequestServices.GetRequiredService<IAuthorizationService>();

            // 权限验证 这里将调用NgPermissionHandler,失败之后将进行中断输出
            var authorizationResult = await authorizationService
                .AuthorizeAsync(context.HttpContext.User, null, new MbpPermissionRequirement(ActionCodes));
            if (!authorizationResult.Succeeded)
            {
                context.HttpContext.Response.ContentType = "application/json";
                context.Result = new JsonResult(new { state = 4001, message = $"访问者缺乏权限:{string.Join(",", ActionCodes)}", version = "1", content = new List<object>() });
            }
        }
    }
}
