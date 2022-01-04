using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Mbp.AspNetCore.Api.Filters;

namespace Res.Service
{
    public class SampleFilter : MbpFilter
    {
        private ILogger<SampleFilter> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        public SampleFilter(ILogger<SampleFilter> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public async override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            _logger.LogInformation("DemoService 过滤器");
            await next();
        }
    }
}
