using Ins.Sample.Business;
using Ins.Sample.Service.GrpcClient;
using Microsoft.Extensions.DependencyInjection;
using WuhanIns.Nitrogen.Configuration;
using WuhanIns.Nitrogen.Ddd.Application.ObjectMapper.AutoMapper;
using WuhanIns.Nitrogen.Net;

namespace Ins.Sample.Service
{
    public class InsDemoServiceModule: InsDemoBussinessModule
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
            services.AddNitrogenGrpcClient<Greeter.GreeterClient>("Ng-Demo-2");

            services.Configure<NitrogenAutoMapperOptions>(options =>
            {
                options.AddMaps<InsDemoServiceModule>();
            });

            return services;
        }
    }
}
