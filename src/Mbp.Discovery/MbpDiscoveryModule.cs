using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using Mbp.Discovery.LoadBalancer;
using Mbp.Discovery.ServiceDiscoveryProvider;
using Mbp.Modular;
using Mbp.Caching;

namespace Mbp.Discovery
{
    /// <summary>
    /// 服务注册/发现模块
    /// </summary>
    [DependsOn(typeof(MbpModule),
        typeof(MbpAspNetModule),typeof(MbpCachingModule))]
    public class MbpDiscoveryModule : MbpAspNetModule
    {
        public override IServiceCollection OnPreConfigureServices(IServiceCollection services)
        {
            // 代办:这里获取配置来源需要替换，这里固化从appsetting.json中获取了
            services.Configure<DiscoveryModuleOptions>(services.BuildServiceProvider().GetService<IConfiguration>().GetSection("Mbp:Discovery"));

            return services;
        }

        public override IServiceCollection ConfigureServices(IServiceCollection services)
        {
            // 获取当前模块配置
            var discoveryOptions = services.BuildServiceProvider().GetService<IOptions<DiscoveryModuleOptions>>()?.Value;

            // 服务发现策略相关注册，集群和非集群模式
            if (discoveryOptions != null && discoveryOptions.IsUseServiceRegistry)
            {
                if (string.IsNullOrEmpty(discoveryOptions.LoadBalancePolicy))
                {
                    discoveryOptions.LoadBalancePolicy = "Random";
                    var logger = services.BuildServiceProvider().GetService<ILogger<MbpDiscoveryModule>>();
                    logger.LogWarning("客户端服务发现的负载均衡策略未指定，系统将默认配置为[Random]策略");
                }

                // 客户端服务发现负载均衡策略
                switch (discoveryOptions.LoadBalancePolicy)
                {
                    case "RoundRobin":
                        services.AddSingleton(typeof(ILoadBalancer), typeof(RoundRobinLoadBalancer));
                        break;
                    case "Random":
                        services.AddSingleton(typeof(ILoadBalancer), typeof(RandomLoadBalancer));
                        break;
                    default:
                        services.AddSingleton(typeof(ILoadBalancer), typeof(RandomLoadBalancer));
                        break;
                }

                // 注册微服务集群模式下的服务发现策略
                services.AddSingleton(typeof(IDiscovery), typeof(ClusterDiscoveryService));

                // 注册consul客户端连接工具，使用Http API注册服务
                if (discoveryOptions.DiscoveryProvider == "Consul")
                {
                    // 添加自动注册微服务服务，使用HostService可以在应用程序结束时候做清理工作。
                    services.AddHostedService<MbpAutoRegisterService>();

                    services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(consulConfig =>
                    {
                        consulConfig.Address = new Uri(services.BuildServiceProvider().GetService<IOptions<DiscoveryModuleOptions>>().Value.DiscoveryCenter);
                    }));

                    // 注册服务注册/发现提供程序服务，可以扩展多个提供程序实现
                    services.AddSingleton(typeof(IServiceDiscoveryProvider), typeof(ConsulServiceProvider));
                }
            }
            else
            {
                // 如果没有开启服务注册中心，那么将启用本地配置的远程服务地址，这种服务发现模式不会对远程做负载。
                services.AddSingleton(typeof(IDiscovery), typeof(SingletonDiscoveryService));
            }

            // 健康检查
            services.AddHealthChecks();

            // 注册服务发现客户端
            services.AddSingleton(typeof(IMbpDiscovery), typeof(DiscoveryClient));

            return services;
        }

        public override void OnModuleInitialization(IApplicationBuilder app)
        {
            // 健康检查
            app.UseHealthChecks("/ng-well");
        }
    }
}
