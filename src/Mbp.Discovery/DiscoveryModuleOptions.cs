using System.Collections.Generic;

namespace Mbp.Discovery
{
    /// <summary>
    /// 发现模块选项
    /// </summary>
    public class DiscoveryModuleOptions
    {
        /// <summary>
        /// 服务注册/发现中心
        /// </summary>
        public string DiscoveryCenter { get; set; }

        /// <summary>
        /// 微服务名称
        /// </summary>
        public string MicroServiceName { get; set; }

        /// <summary>
        /// 微服务分组名称
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// 是否使用服务注册中心
        /// </summary>
        public bool IsUseServiceRegistry { get; set; }

        /// <summary>
        /// 服务注册/发现中心提供程序，默认为Consul
        /// </summary>
        public string DiscoveryProvider { get; set; }

        /// <summary>
        /// 服务发现负载均衡策略
        /// </summary>
        public string LoadBalancePolicy { get; set; }

        /// <summary>
        /// 手动配置远程服务列表，当不使用服务中心的时候
        /// </summary>
        public Dictionary<string, RemoteService> RemoteServices { get; set; }
    }
}
