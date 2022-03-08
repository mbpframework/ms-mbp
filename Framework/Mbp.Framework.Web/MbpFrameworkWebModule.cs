using Mbp.AspNetCore;
using Mbp.Caching;
using Mbp.Config.Apollo;
using Mbp.DataAccess;
using Mbp.Ddd;
using Mbp.Ddd.Application.ObjectMapper.AutoMapper;
using Mbp.Discovery;
using Mbp.Framework.Application;
using Mbp.Framework.Web.GrpcClient;
using Mbp.Logging;
using Mbp.Modular;
using Mbp.Net;
using Mbp.Framework.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mbp.Framework.Web
{
    /// <summary>
    /// Mbp平台启动模块
    /// </summary>
    [DependsOn(typeof(MbpFrameworkApplicationModule), typeof(MbpAspNetModule), 
        typeof(MbpCachingModule),
        typeof(MbpApolloModule),
        typeof(MbpDddModule),
        typeof(MbpDiscoveryModule),
        typeof(MbpLoggerModule),
        typeof(MbpNetModule),
        typeof(MbpDataAccessModule),
        typeof(MbpWebModule)
        )]
    public class MbpFrameworkWebModule : MbpModule
    {
        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public override IServiceCollection ConfigureServices(IServiceCollection services)
        {
            // 注册产品配置的选项服务
            ConfigureProductOptions<ProductConfig>(services, "Sample");

            // 注册其他微服务地址
            services.AddMbpGrpcClient<Greeter.GreeterClient>("Ng-Demo-2");

            services.Configure<MbpAutoMapperOptions>(options =>
            {
                options.AddMaps<MbpFrameworkWebModule>();
            });

            return services;
        }

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
