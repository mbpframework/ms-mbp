using Mbp.AspNetCore;
using Mbp.Caching;
using Mbp.Config.Apollo;
using Mbp.Configuration;
using Mbp.DataAccess;
using Mbp.Ddd;
using Mbp.Discovery;
using Mbp.Logging;
using Mbp.Modular;
using Mbp.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mbp.WebHost
{
    /// <summary>
    /// Mbp平台启动模块
    /// </summary>
    [DependsOn(typeof(MbpAspNetModule),
        typeof(MbpNetModule),
        typeof(MbpCachingModule),
        typeof(MbpApolloModule),
        typeof(MbpDddModule),
        typeof(MbpDiscoveryModule),
        typeof(MbpLoggerModule),
        typeof(MbpNetModule),
        typeof(MbpDataAccessModule),
        typeof(MbpWebModule)
        )]
    public class MbpWebHostModule : MbpModule
    {
        public override IServiceCollection OnPreConfigureServices(IServiceCollection services)
        {
            var config = new ConfigurationBuilder()
                   .SetBasePath(AppContext.BaseDirectory)
                   .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                   .AddEnvironmentVariables()
                   .Build();

            // 全局配置
            services.Configure<GlobalSetting>(config.GetSection("Mbp"));

            // 业务产品通用配置
            services.Configure<ProductSetting>(services.BuildServiceProvider().GetService<IConfiguration>().GetSection("Mbp:ProductSettings"));

            return services;
        }

        /// <summary>
        /// 注册NgWeb模块服务
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public override IServiceCollection ConfigureServices(IServiceCollection services)
        {
            return services;
        }
    }
}
