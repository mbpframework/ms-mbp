using Microsoft.Extensions.Options;
using WuhanIns.Nitrogen.Discovery.LoadBalancer;
using WuhanIns.Nitrogen.Discovery.ServiceDiscoveryProvider;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace WuhanIns.Nitrogen.Discovery.LoadBalancer
{
    /// <summary>
    /// 服务发现负载均衡策略工厂
    /// </summary>
    internal class LoadBalanceFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public LoadBalanceFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ILoadBalancer Create()
        {
            return _serviceProvider.GetService<ILoadBalancer>();
        }
    }
}
