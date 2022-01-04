using System.Threading.Tasks;

namespace Mbp.Discovery.LoadBalancer
{
    public interface ILoadBalancer
    {
        /// <summary>
        /// ��ȡ΢����
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        Task<string> GetMicroWebApiServiceAsync(string serviceName);

        /// <summary>
        /// ��ȡ΢����
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        Task<string> GetMicroGrpcServiceAsync(string serviceName);
    }
}