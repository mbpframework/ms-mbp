using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Mbp.Discovery
{
    /// <summary>
    /// 单体部署下的服务注册/发现
    /// </summary>
    internal class SingletonDiscoveryService : IDiscovery
    {
        private readonly IOptions<DiscoveryModuleOptions> _options;

        public SingletonDiscoveryService(IOptions<DiscoveryModuleOptions> options)
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
