using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mbp.Modular;
using Mbp.EventBus;
using Mbp.EventBus.DistributedTransaction;

namespace Mbp.EventBus
{
    /// <summary>
    /// 事件总线模块
    /// </summary>
    [DependsOn(typeof(MbpModule))]
    public class MbpEventBusModule : MbpModule
    {
        public override IServiceCollection OnPreConfigureServices(IServiceCollection services)
        {
            // 代办:这里获取配置来源需要替换，这里固化从appsetting.json中获取了
            services.Configure<EventBusModuleOptions>(services.BuildServiceProvider().GetService<IConfiguration>().GetSection("Mbp:EventBus"));

            return services;
        }

        public override IServiceCollection ConfigureServices(IServiceCollection services)
        {
            // 注册事件总线配置
            services.AddMbpEventBus();

            // 注册事件总线服务 单例
            services.AddSingleton(typeof(IMbpEventBus), typeof(MbpEventBus));

            // 注册分布式事务服务 单例
            services.AddSingleton(typeof(IMbpDistributedTransaction), typeof(MbpDistributedTransaction));

            return services;
        }
    }
}
