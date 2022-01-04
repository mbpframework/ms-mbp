using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Mbp.Discovery;
using Mbp.Net.gRPC;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MbpGrpcClientServiceExtensions
    {
        /// <summary>
        /// 注册Mbp gRPC客户端
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services"></param>
        /// <param name="serviceName"></param>
        public static void AddMbpGrpcClient<T>(this IServiceCollection services, string serviceName) where T : class
        {
            // 切换为非安全模式调用，及不适用LTS，内部网络不建议，也不需要使用LTS
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            // 添加gRPC客户端，并集成polly，重试和熔断机制
            services.AddGrpcClient<T>(o =>
            {
                // 发现客户端地址
                o.Address = new Uri(services.BuildServiceProvider().GetService<IMbpDiscovery>().GetGrpcServiceUrl(serviceName));

                // gRPC客户端拦截
                o.Interceptors.Add(new GrpcClientInterceptor(services));
            }).AddPolicyHandler(GetRetryPolicy()).AddPolicyHandler(GetCircuitBreakerPolicy())
            ;
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
