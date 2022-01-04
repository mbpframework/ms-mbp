using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace WuhanIns.Nitrogen.Discovery.ServiceDiscoveryProvider
{
    /// <summary>
    /// 服务注册发现中心提供程序，默认为Consul
    /// </summary>
    internal class DiscoveryProviderFactory
    {
        /// <summary>
        /// 根据配置创建提供程序实例并返回
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        internal IServiceDiscoveryProvider Create(IServiceProvider serviceProvider)
        {
            return serviceProvider.GetService<IServiceDiscoveryProvider>();
        }
    }
}
