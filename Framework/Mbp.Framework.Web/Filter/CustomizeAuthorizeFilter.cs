using Mbp.Core.User;
using Mbp.AspNetCore.Api.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mbp.Framework.Web.Filter
{
    /// <summary>
    /// 根据特性头检查身份信息
    /// </summary>
    public class CustomizeAuthorizeFilter : MbpFilter
    {
        private readonly ICurrentUser _currentUser;

        public CustomizeAuthorizeFilter(ICurrentUser currentUser)
        {
            _currentUser = currentUser;
        }

        public async override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var controllerAuthorize = context.Controller.GetType().GetCustomAttributes(true).OfType<AuthorizeAttribute>()
              .Any();
            var actionAuthorize = ((ControllerActionDescriptor)context.ActionDescriptor).MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any();
            var actionAllowAnonymous = ((ControllerActionDescriptor)context.ActionDescriptor).MethodInfo.GetCustomAttributes(true).OfType<AllowAnonymousAttribute>().Any();

            if (IsNeedAuthroize(controllerAuthorize, actionAuthorize, actionAllowAnonymous))
            {
                if (!string.IsNullOrEmpty(_currentUser.AccessToken) && !string.IsNullOrEmpty(_currentUser.UserId))
                {
                    await next();
                }
                else
                {
                    context.HttpContext.Response.ContentType = "application/json";
                    await context.HttpContext.Response.WriteAsync(Newtonsoft.Json.JsonConvert.SerializeObject(new { state = 4001, message = "身份验证失败", version = 1, content = new List<object>() }));
                }
            }
            else
            {
                await next();
            }
        }

        // 判断接口方法是否需要身份验证
        private bool IsNeedAuthroize(bool controllerAuthorize, bool actionAuthorize, bool actionAllowAnonymous)
        {
            if (actionAuthorize)// 1.方法显示声明需要验证身份
            {
                return true;
            }
            else if (controllerAuthorize && !actionAllowAnonymous)// 2.控制器显示声明需要身份验证，action上无匿名访问声明
            {
                return true;
            }

            return false;
        }
    }
}
