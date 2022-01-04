using System.Collections.Generic;
using System.Threading.Tasks;
using Mbp.Discovery.ServiceDiscoveryProvider;

namespace Mbp.Discovery.LoadBalancer
{
    /// <summary>
    /// ÂÖÑ¯¸ºÔØ¾ùºâ²ßÂÔ
    /// </summary>
    public class RoundRobinLoadBalancer : ILoadBalancer
    {
        private readonly IServiceDiscoveryProvider _sdProvider;

        public RoundRobinLoadBalancer(IServiceDiscoveryProvider sdProvider)
        {
            _sdProvider = sdProvider;
        }

        private readonly object _lock = new object();
        private int _index = 0;

        public async Task<string> GetMicroWebApiServiceAsync(string serviceName)
        {
            var services = await _sdProvider.GetMicroWebApiServicesAsync(serviceName);
            return LoadBalance(services);
        }

        public async Task<string> GetMicroGrpcServiceAsync(string serviceName)
        {
            var services = await _sdProvider.GetMicroGrpcServicesAsync(serviceName);
            return LoadBalance(services);
        }

        private string LoadBalance(List<string> services)
        {
            if (services.Count <= 0)
                return string.Empty;

            lock (_lock)
            {
                if (_index >= services.Count)
                {
                    _index = 0;
                }
                return services[_index++];
            }
        }
    }
}