using Mbp.Core.User;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Mbp.AspNetCore.Permission
{
    /// <summary>
    /// 权限处理逻辑
    /// </summary>
    public class MbpPermissionHandler : AuthorizationHandler<MbpPermissionRequirement>
    {
        public IAuthenticationSchemeProvider Schemes { get; set; }

        public IHttpContextAccessor HttpContextAccessor { get; set; }

        public ICurrentUser CurrentUser { get; set; }

        public MbpPermissionHandler(IAuthenticationSchemeProvider schemes,
            IHttpContextAccessor httpContextAccessor, ICurrentUser currentUser)
        {
            Schemes = schemes;
            HttpContextAccessor = httpContextAccessor;
            CurrentUser = currentUser;
        }

        /// <summary>
        /// 权限扩展提供程序
        /// </summary>
        /// <param name="context"></param>
        /// <param name="requirement"></param>
        /// <returns></returns>
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MbpPermissionRequirement requirement)
        {
            var httpContext = HttpContextAccessor.HttpContext;

            var isAuthenticated = httpContext.User.Identity.IsAuthenticated;

            // 身份验证通过
            if (isAuthenticated)
            {
                // 代办 这里可以扩展操作权限编码验证通过后的处理
                var actionCodes = requirement.ActionCodes;

                // 代办 取出用户的所有操作权限来进行比较
                //var isSuccess = true;

                //if (isSuccess)
                //{
                //    context.Succeed(requirement);
                //}
                //else
                //{
                //    context.Fail();
                //}
            }

            return Task.CompletedTask;
        }
    }
}
