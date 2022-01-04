using Mbp.Dependency;

namespace Mbp.Discovery
{
    /// <summary>
    /// 服务发现服务
    /// </summary>
    public interface IMbpDiscovery : ISingletonDependency
    {
        /// <summary>
        /// 通过服务名查找WebApi服务侦听地址
        /// </summary>
        /// <param name="serviceName">服务名</param>
        /// <returns></returns>
        string GetWebApiServiceUrl(string serviceName);

        /// <summary>
        /// 通过服务名查找gRPC服务侦听地址
        /// </summary>
        /// <param name="serviceName">服务名</param>
        /// <returns></returns>
        string GetGrpcServiceUrl(string serviceName);
    }
}
