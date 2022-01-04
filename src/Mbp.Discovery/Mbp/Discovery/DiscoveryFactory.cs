using Microsoft.Extensions.Options;
using WuhanIns.Nitrogen.Web.Convention;
using WuhanIns.Nitrogen.Discovery.ServiceDiscoveryProvider;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace WuhanIns.Nitrogen.Discovery
{
    /// <summary>
    /// 服务发现/注册提供类工厂
    /// </summary>
    internal class DiscoveryFactory
    {
        private IDiscovery _ngDiscovery = null;

        public DiscoveryFactory(IOptions<DiscoveryModuleOptions> options, IServiceProvider serviceProvider)
        {
            // 使用何种注册/发现方式由框架使用者自由选择
            if (options.Value.IsUseServiceRegistry)
                _ngDiscovery = new ClusterDiscoveryService(options, serviceProvider);
            else
                _ngDiscovery = new SingletonDiscoveryService(options);
        }

        internal IDiscovery Create()
        {
            return _ngDiscovery;
        }
    }
}
