using Mbp.Discovery.LoadBalancer;
using System.Threading.Tasks;

namespace Mbp.Discovery
{
    /// <summary>
    /// 集群部署模式下的服务注册/发现
    /// </summary>
    internal class ClusterDiscoveryService : IDiscovery
    {
        private readonly ILoadBalancer _loadBalancer;

        public ClusterDiscoveryService(ILoadBalancer loadBalancer)
        {
            _loadBalancer = loadBalancer;
        }

        /// <summary>
        /// 根据微服务名称获取微服务实例地址
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public async Task<string> GetWebApiServiceUrlAsync(string serviceName)
        {
            return await _loadBalancer.GetMicroWebApiServiceAsync(serviceName);
        }

        /// <summary>
        /// 根据微服务名称获取微服务实例地址
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public async Task<string> GetGrpcServiceUrlAsync(string serviceName)
        {
            return await _loadBalancer.GetMicroGrpcServiceAsync(serviceName);
        }
    }
}
