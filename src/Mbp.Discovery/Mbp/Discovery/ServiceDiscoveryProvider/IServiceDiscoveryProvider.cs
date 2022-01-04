using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mbp.Discovery.ServiceDiscoveryProvider
{
    /// <summary>
    /// ��ȡ΢�����ṩ����ӿ�
    /// </summary>
    public interface IServiceDiscoveryProvider
    {
        /// <summary>
        /// ����΢�������ƻ�ȡ΢����ʵ������
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        Task<List<string>> GetMicroWebApiServicesAsync(string serviceName);

        /// <summary>
        /// ����΢�������ƻ�ȡ΢����ʵ������
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        Task<List<string>> GetMicroGrpcServicesAsync(string serviceName);
    }
}