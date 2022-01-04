using Mbp.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;

namespace Mbp.AspNetCore.Api.Filters
{
    /// <summary>
    /// 请求响应数据格式处理中间件,支持继承重写
    /// </summary>
    internal class ResponseMiddlewareAttribute : ActionFilterAttribute
    {
        private readonly IMbpContextAccessor _MbpContextAccessor;
        public ResponseMiddlewareAttribute(IMbpContextAccessor MbpContextAccessor)
        {
            _MbpContextAccessor = MbpContextAccessor;
        }

        /// <summary>
        /// 格式化响应数据
        /// </summary>
        /// <param name="context"></param>
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Result is JsonResult)
            {
                var jsonResult = context.Result as JsonResult;

                context.Result = new JsonResult(new { state = 200, message = "正常", version = context.RouteData.Values["version"], content = jsonResult.Value });
            }
            else if (context.Result is ObjectResult)
            {
                var objectResult = context.Result as ObjectResult;

                context.Result = new JsonResult(new { state = 200, message = "正常", version = context.RouteData.Values["version"], content = objectResult.Value });
            }
            else if (context.Result is EmptyResult)
            {
                context.Result = new ObjectResult(new { state = 200, message = "正常", version = context.RouteData.Values["version"], content = new List<object>() });
            }
            else if (context.Result is ContentResult)
            {
                context.Result = new ObjectResult(new { state = 200, message = "正常", version = context.RouteData.Values["version"], content = (context.Result as ContentResult).Content });
            }
            else if (context.Result is StatusCodeResult)
            {
                context.Result = new ObjectResult(new { state = (context.Result as StatusCodeResult).StatusCode, message = "", version = context.RouteData.Values["version"], content = new List<object>() });
            }
        }
    }
}
