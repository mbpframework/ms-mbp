using Mbp.Discovery.ServiceDiscoveryProvider;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Mbp.Discovery.LoadBalancer
{
    /// <summary>
    /// ��������ؾ������
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
            // ��ȡ΢�����б�
            var services = await _sdProvider.GetMicroWebApiServicesAsync(serviceName);
            return LoadBalance(services);
        }

        public async Task<string> GetMicroGrpcServiceAsync(string serviceName)
        {
            // ��ȡ΢�����б�
            var services = await _sdProvider.GetMicroGrpcServicesAsync(serviceName);
            return LoadBalance(services);
        }

        private string LoadBalance(List<string> services)
        {
            if (services.Count <= 0)
                return string.Empty;

            // ��΢�����б������ѡ��һ��
            return services[_random.Next(services.Count)];
        }
    }
}