using Mbp.Framework.Contracts;
using Mbp.Framework.Domain;
using Mbp.Framework.Domain.Share;
using Mbp.Modular;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mbp.Framework.Application
{
    /// <summary>
    /// 应用层继承
    /// </summary>
    [DependsOn(typeof(MbpFrameworkApplicationContractsModule),
        typeof(MbpFrameworkDomainShareModule),
        typeof(MbpFrameworkDomainModule))]
    public class MbpFrameworkApplicationModule : MbpModule
    {
        public void ConfigureOptions<T>(IServiceCollection services) where T : class
        {
            // 从IOC中提取配置服务
            var configuration = services.BuildServiceProvider().GetService<IConfiguration>().GetSection("Mbp");

            // 示例：注册配置选项服务
            services.Configure<T>(configuration);
        }

        public void ConfigureProductOptions<T>(IServiceCollection services, string productName) where T : class
        {
            // 从IOC中提取配置服务
            var configuration = services.BuildServiceProvider().GetService<IConfiguration>().GetSection($"Mbp:ProductSettings:ProductConfigs:Mbp.{productName}");

            // 示例：注册配置选项服务
            services.Configure<T>(configuration);
        }
    }
}
