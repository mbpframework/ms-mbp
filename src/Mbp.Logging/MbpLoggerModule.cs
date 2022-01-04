using Exceptionless;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mbp.Modular;

namespace Mbp.Logging
{
    /// <summary>
    /// 日志模块
    /// </summary>
    [DependsOn(typeof(MbpModule))]
    public class MbpLoggerModule : MbpAspNetModule
    {
        public override IServiceCollection OnPreConfigureServices(IServiceCollection services)
        {
            // 代办:这里获取配置来源需要替换，这里固化从appsetting.json中获取了
            services.Configure<LoggerModuleOptions>(services.BuildServiceProvider().GetService<IConfiguration>().GetSection("Mbp:Logger"));
            return services;
        }

        public override IServiceCollection ConfigureServices(IServiceCollection services)
        {
            return services;
        }
    }
}
