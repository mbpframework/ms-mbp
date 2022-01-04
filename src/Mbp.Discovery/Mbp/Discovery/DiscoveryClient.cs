namespace Mbp.Discovery
{
    /// <summary>
    /// 服务注册/发现提供服务 单例注册
    /// </summary>
    public class DiscoveryClient : IMbpDiscovery
    {
        private readonly IDiscovery _discovery;

        public DiscoveryClient(IDiscovery discovery)
        {
            _discovery = discovery;
        }

        public string GetWebApiServiceUrl(string serviceName)
        {
            return _discovery.GetWebApiServiceUrlAsync(serviceName).Result;
        }

        public string GetGrpcServiceUrl(string serviceName)
        {
            return _discovery.GetGrpcServiceUrlAsync(serviceName).Result;
        }
    }
}
