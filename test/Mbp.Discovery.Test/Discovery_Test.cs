using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shouldly;
using System.Threading.Tasks;
using Mbp.Discovery;
using Xunit;

namespace Mbp.Discovery.Test
{
    public class Discovery_Test
    {
        private readonly IMbpDiscovery _ngDiscovery;

        public Discovery_Test()
        {
            var services = new ServiceCollection();
            services.AddSingleton(typeof(IDiscovery), typeof(SingletonDiscoveryService_Test));
            services.AddSingleton(typeof(IMbpDiscovery), typeof(DiscoveryClient));

            DiscoveryModuleOptions discoveryModuleOptions = new DiscoveryModuleOptions()
            {
                MicroServiceName = "Ng-Demo",
                IsUseServiceRegistry = false,
                DiscoveryProvider = "",
                RemoteServices = new System.Collections.Generic.Dictionary<string, RemoteService>()
                {
                    { "Ng-Demo-2-5009",new RemoteService(){ Host="localhost", HttpPort=5009, GrpcPort=50092 } }
                }
            };

            services.AddSingleton(Options.Create<DiscoveryModuleOptions>(discoveryModuleOptions));

            _ngDiscovery = services.BuildServiceProvider().GetService<IMbpDiscovery>();
        }

        [Fact]
        public void GetWebApiServiceUrl_Test()
        {
            _ngDiscovery.GetWebApiServiceUrl("Ng-Demo-2-5009").ShouldBe("http://localhost:5009/");
        }

        [Fact]
        public void GetGrpcServiceUrl_Test()
        {
            _ngDiscovery.GetGrpcServiceUrl("Ng-Demo-2-5009").ShouldBe("http://localhost:50092/");
        }
    }

    /// <summary>
    /// 单体部署下的服务注册/发现
    /// </summary>
    internal class SingletonDiscoveryService_Test : IDiscovery
    {
        private readonly IOptions<DiscoveryModuleOptions> _options;

        public SingletonDiscoveryService_Test(IOptions<DiscoveryModuleOptions> options)
        {
            _options = options;
        }

        /// <summary>
        /// 获取WebApi服务地址（不启用注册中心场景下）
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public Task<string> GetWebApiServiceUrlAsync(string serviceName)
        {
            // 简单的部署模式下，远程服务没有做负载，通信模式为模块间的通信，只要维护本地的注册地址列表即可
            var remoteServiceList = _options.Value.RemoteServices;
            return Task.Run(() => $"http://{remoteServiceList[serviceName]?.Host}:{remoteServiceList[serviceName]?.HttpPort}/");
        }

        /// <summary>
        /// 获取gRPC服务地址（不启用注册中心场景下）
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public Task<string> GetGrpcServiceUrlAsync(string serviceName)
        {
            // 简单的部署模式下，远程服务没有做负载，通信模式为模块间的通信，只要维护本地的注册地址列表即可
            var remoteServiceList = _options.Value.RemoteServices;
            return Task.Run(() => $"http://{remoteServiceList[serviceName]?.Host}:{remoteServiceList[serviceName]?.GrpcPort}/");
        }
    }
}
