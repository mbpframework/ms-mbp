using Consul;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mbp.Configuration;
using Mbp.Discovery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Mbp.Discovery
{
    /// <summary>
    /// Mbp服务注册服务，用于在启用了Consul注册/发现中心之后自动将本微服务注册到Consul中
    /// </summary>
    public class MbpAutoRegisterService : IHostedService
    {
        private readonly IConsulClient _consulClient;
        private readonly ILogger _logger;
        private readonly IServer _server;
        private readonly IOptions<GlobalSetting> _globalOptions;
        private readonly IOptions<DiscoveryModuleOptions> _discoveryOptions;

        private CancellationTokenSource _cts;
        private string _serviceId;

        public MbpAutoRegisterService(IConsulClient consulClient, ILogger<MbpAutoRegisterService> logger, IServer server, IOptions<GlobalSetting> globalOptions, IOptions<DiscoveryModuleOptions> discoveryOptions)
        {
            _consulClient = consulClient;
            _logger = logger;
            _server = server;
            _globalOptions = globalOptions;
            _discoveryOptions = discoveryOptions;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            var features = _server.Features;
            string address = string.Empty;
            if (features.Get<IServerAddressesFeature>().PreferHostingUrls)
            {
                address = features.Get<IServerAddressesFeature>().Addresses.First();
            }
            else
            {
                var host = Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(address => address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)?.ToString();
                address = $"http://{host}:{_globalOptions.Value.HttpPort}";
            }
            var uri = new Uri(address);

            _serviceId = "Ng-Service-" + Dns.GetHostName() + "-" + uri.Authority;

            var registration = new AgentServiceRegistration()
            {
                ID = _serviceId,
                Name = _discoveryOptions.Value.MicroServiceName,
                Address = uri.Host,
                Port = uri.Port,
                Tags = new[] { "Ng-MicroService", _discoveryOptions.Value.GroupName },
                Meta = new Dictionary<string, string>() { { "gRpcPort", _globalOptions.Value.GrpcPort } },
                Check = new AgentServiceCheck()
                {
                    HTTP = $"{uri.Scheme}://{uri.Host}:{uri.Port}/ng-well",
                    Timeout = TimeSpan.FromSeconds(2),
                    Interval = TimeSpan.FromSeconds(10)
                }
            };

            _logger.LogInformation("Registering in Consul");

            // 首先移除服务，避免重复注册
            await _consulClient.Agent.ServiceDeregister(registration.ID, _cts.Token);
            await _consulClient.Agent.ServiceRegister(registration, _cts.Token);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _cts.Cancel();
            _logger.LogInformation("Deregistering from Consul");
            try
            {
                await _consulClient.Agent.ServiceDeregister(_serviceId, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Deregisteration failed");
            }
        }
    }
}
