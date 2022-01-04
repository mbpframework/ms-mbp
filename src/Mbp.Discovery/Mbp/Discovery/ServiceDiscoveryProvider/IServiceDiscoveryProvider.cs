using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mbp.Discovery.ServiceDiscoveryProvider
{
    /// <summary>
    /// 获取微服务提供程序接口
    /// </summary>
    public interface IServiceDiscoveryProvider
    {
        /// <summary>
        /// 根据微服务名称获取微服务实例集合
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        Task<List<string>> GetMicroWebApiServicesAsync(string serviceName);

        /// <summary>
        /// 根据微服务名称获取微服务实例集合
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        Task<List<string>> GetMicroGrpcServicesAsync(string serviceName);
    }
}