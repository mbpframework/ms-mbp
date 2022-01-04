using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Mbp.AspNetCore.Api.Filters
{
    /// <summary>
    /// Mbp抽象Action Filter
    /// </summary>
    public abstract class MbpFilter : IAsyncActionFilter
    {
        public abstract Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next);
    }
}
