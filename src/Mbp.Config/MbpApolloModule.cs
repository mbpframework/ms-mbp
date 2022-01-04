using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Mbp.Modular;
using System;

namespace Mbp.Config.Apollo
{
    /// <summary>
    /// 配置模块
    /// </summary>
    [DependsOn(typeof(MbpModule))]
    public class MbpApolloModule : MbpModule
    {
        public override IServiceCollection ConfigureServices(IServiceCollection services)
        {
            return services;
        }
    }
}
