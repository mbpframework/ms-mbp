using Mbp.Discovery;
using Mbp.Logging;
using Mbp.Modular;
using Mbp.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using System;
using System.Net;
using System.Net.Http;

namespace Mbp.Net
{
    /// <summary>
    /// 网络通信模块
    /// </summary>
    [DependsOn(typeof(MbpModule),
        typeof(MbpLoggerModule),
        typeof(MbpDiscoveryModule))]
    public class MbpNetModule : MbpModule
    {
        public override IServiceCollection OnPreConfigureServices(IServiceCollection services)
        {
            // 代办:这里获取配置来源需要替换，这里固化从appsetting.json中获取了
            services.Configure<NetModuleOptions>(services.BuildServiceProvider().GetService<IConfiguration>().GetSection("Mbp:Net"));

            return services;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public override IServiceCollection ConfigureServices(IServiceCollection services)
        {
            // 默认注册gRPC服务端
            services.AddGrpc(o =>
            {
                // gRPC服务端设置
            });

            services.AddScoped(typeof(IMbpHttpClient), typeof(MbpHttpClient));
            services.AddScoped(typeof(IMbpHttpClientMicro), typeof(MbpHttpClientMicro));
            services.AddScoped(typeof(IMbpHttpClientRaw), typeof(MbpHttpClientRaw));
            services.AddScoped(typeof(IHttpClientService), typeof(HttpClientService));

            services.AddHttpClient("WebApi", (provider,client) =>
            {
               // 注意：因为发起了http请求，这里不能使用注册为scope依赖项 
            }).ConfigurePrimaryHttpMessageHandler(o => new HttpClientHandler()
            {
                // 解压缩
                AutomaticDecompression = DecompressionMethods.All
            }).AddPolicyHandler(GetRetryPolicy()).AddPolicyHandler(GetCircuitBreakerPolicy());

            return services;
        }

        private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
        }

        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .RetryAsync(5);
        }
    }
}
