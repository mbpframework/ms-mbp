using System.Threading.Tasks;

namespace Mbp.Discovery.LoadBalancer
{
    public interface ILoadBalancer
    {
        /// <summary>
        /// 获取微服务
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        Task<string> GetMicroWebApiServiceAsync(string serviceName);

        /// <summary>
        /// 获取微服务
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        Task<string> GetMicroGrpcServiceAsync(string serviceName);
    }
}