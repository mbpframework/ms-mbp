using Consul;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WuhanIns.Nitrogen.Caching;

namespace WuhanIns.Nitrogen.Discovery
{
    /// <summary>
    /// 服务发现客户端代理
    /// </summary>
    public class NgAutoDiscoveryService : IHostedService, IDisposable
    {
        private int executionCount = 0;
        private readonly ILogger<NgAutoDiscoveryService> _logger;
        private Timer _timer;
        private readonly INgCache _ngCache;
        private readonly IConsulClient _consulClient;

        public NgAutoDiscoveryService(ILogger<NgAutoDiscoveryService> logger, INgCache ngCache, IConsulClient consulClient)
        {
            _logger = logger;
            _ngCache = ngCache;
            _consulClient = consulClient;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service running.");

            _timer = new Timer(DiscoveryMicroServices, null, TimeSpan.Zero, TimeSpan.FromSeconds(3));

            return Task.CompletedTask;
        }

        private void DiscoveryMicroServices(object state)
        {
            var count = Interlocked.Increment(ref executionCount);

            _logger.LogInformation("Timed Hosted Service is working. Count: {Count}", count);
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

            return queryResult;
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
