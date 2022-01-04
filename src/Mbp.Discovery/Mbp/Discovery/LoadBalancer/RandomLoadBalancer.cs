using Mbp.Discovery.ServiceDiscoveryProvider;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Mbp.Discovery.LoadBalancer
{
    /// <summary>
    /// 随机数负载均衡策略
    /// </summary>
    public class RandomLoadBalancer : ILoadBalancer
    {
        private readonly IServiceDiscoveryProvider _sdProvider;

        public RandomLoadBalancer(IServiceDiscoveryProvider sdProvider)
        {
            _sdProvider = sdProvider;
        }

        private Random _random = new Random();

        public async Task<string> GetMicroWebApiServiceAsync(string serviceName)
        {
            // 获取微服务列表
            var services = await _sdProvider.GetMicroWebApiServicesAsync(serviceName);
            return LoadBalance(services);
        }

        public async Task<string> GetMicroGrpcServiceAsync(string serviceName)
        {
            // 获取微服务列表
            var services = await _sdProvider.GetMicroGrpcServicesAsync(serviceName);
            return LoadBalance(services);
        }

        private string LoadBalance(List<string> services)
        {
            if (services.Count <= 0)
                return string.Empty;

            // 从微服务列表中随机选择一个
            return services[_random.Next(services.Count)];
        }
    }
}