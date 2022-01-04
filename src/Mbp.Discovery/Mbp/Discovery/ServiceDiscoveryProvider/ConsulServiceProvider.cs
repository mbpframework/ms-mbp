using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Consul;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mbp.Caching;

namespace Mbp.Discovery.ServiceDiscoveryProvider
{
    /// <summary>
    /// Consul客户端获取微服务提供程序类
    /// 代办（优化）: 在启用注册中心场景下，目前每次进行微服务间通信就会进行一次微服务解析操作，这样会损耗性能。
    /// </summary>
    public class ConsulServiceProvider : IServiceDiscoveryProvider
    {
        private readonly IConsulClient _consulClient;
        private readonly ILogger _logger;
        private readonly IMbpCache _ngCache;

        public ConsulServiceProvider(IConsulClient consulClient, ILogger<ConsulServiceProvider> logger, IMbpCache ngCache)
        {
            _consulClient = consulClient;
            _logger = logger;
            _ngCache = ngCache;
        }

        /// <summary>
        /// 根据微服务名称获取微服务实例集合
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public async Task<List<string>> GetMicroWebApiServicesAsync(string serviceName)
        {
            QueryResult<ServiceEntry[]> queryResult = await GetQueryResultAsync(serviceName);

            var result = new List<string>();
            foreach (var serviceEntry in queryResult.Response)
            {
                result.Add($"http://{serviceEntry.Service.Address}:{serviceEntry.Service.Port}/");
            }
            return result;
        }

        /// <summary>
        /// 根据微服务名称获取微服务实例集合
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public async Task<List<string>> GetMicroGrpcServicesAsync(string serviceName)
        {
            QueryResult<ServiceEntry[]> queryResult = await GetQueryResultFromCacheAsync(serviceName);

            var result = new List<string>();
            foreach (var serviceEntry in queryResult.Response)
            {
                result.Add($"http://{serviceEntry.Service.Address}:{serviceEntry.Service.Meta["gRpcPort"]}/");
            }
            return result;
        }

        private async Task<QueryResult<ServiceEntry[]>> GetQueryResultFromCacheAsync(string serviceName)
        {
            // 从缓存获取
            var queryResult = _ngCache.Get<QueryResult<ServiceEntry[]>>(GetMicroServiceCacheKey(serviceName));

            if (queryResult == null)
            {
                return await GetQueryResultAsync(serviceName);
            }

            return queryResult;
        }

        private async Task<QueryResult<ServiceEntry[]>> GetQueryResultAsync(string serviceName)
        {
            // 根据微服务名字获取可用微服务列表，微服务默认的分组为：Ng-MicroService
            var queryResult = await _consulClient.Health.Service(serviceName, "Ng-MicroService", true);
            var retryTimes = 0;
            while (queryResult.Response.Length == 0 && retryTimes < 3)
            {
                _logger.LogInformation("没有找到可用服务，等待一秒重试....");
                await Task.Delay(1000);
                queryResult = await _consulClient.Health.Service(serviceName, "Ng-MicroService", true);

                // 重试3次 代办:优化代码封装或引入 重试组件
                retryTimes++;
            }

            if (queryResult.Response.Length == 0)
            {
                _logger.LogWarning($"未发现名为：{serviceName} 的微服务实例，请检查部署环境！");
            }

            // 缓存
            _ngCache.Set(GetMicroServiceCacheKey(serviceName), queryResult);

            return queryResult;
        }

        private string GetMicroServiceCacheKey(string serviceName)
        {
            return $"Ng-MicroService-{serviceName}";
        }
    }
}
