using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using WuhanIns.Nitrogen.Web;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Text;

namespace WuhanIns.Nitrogen.Framework.Web
{
    internal class CustomizeHeaderParameterFilter : IOperationFilter
    {
        private readonly IOptions<WebModuleOptions> _options = null;

        public CustomizeHeaderParameterFilter(IOptions<WebModuleOptions> options)
        {
            _options = options;
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
            {
                operation.Parameters = new List<OpenApiParameter>();
            }

            var headers = _options.Value.Headers;
            if (headers == null) return;
            foreach (var header in headers)
            {
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = header.Key,
                    In = ParameterLocation.Header,
                    Required = false,
                    Description = header.Value
                });
            }
        }
    }
}
