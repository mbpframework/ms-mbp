using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Mbp.Discovery
{
    public interface IDiscovery
    {
        /// <summary>
        /// 获取WebApi服务地址（不启用注册中心场景下）
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        Task<string> GetWebApiServiceUrlAsync(string serviceName);

        /// <summary>
        /// 获取gRPC服务地址（不启用注册中心场景下）
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        Task<string> GetGrpcServiceUrlAsync(string serviceName);
    }
}
