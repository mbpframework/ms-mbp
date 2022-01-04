using Res.Business;
using Res.Service.GrpcClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Mbp.Configuration;
using Mbp.Ddd.Application.ObjectMapper.AutoMapper;
using Mbp.Net;

namespace Res.Service
{
    public class InsSampleServiceModule : InsSampleBussinessModule
    {
        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public override IServiceCollection ConfigureServices(IServiceCollection services)
        {
            // 注册产品配置的选项服务
            base.ConfigureProductOptions<ProductConfig>(services, "Sample");

            // 注册其他微服务地址
            services.AddMbpGrpcClient<Greeter.GreeterClient>("Ng-Demo-2");

            services.Configure<MbpAutoMapperOptions>(options =>
            {
                options.AddMaps<InsSampleServiceModule>();
            });

            return services;
        }
    }
}
