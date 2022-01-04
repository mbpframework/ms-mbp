using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Mbp.Modular;
using System;
using Mbp.Caching;

namespace Mbp.Caching
{
    /// <summary>
    /// 缓存模块
    /// </summary>
    [DependsOn(typeof(MbpModule))]
    public class MbpCachingModule : MbpModule
    {
        private string _cacheProvider;

        public override IServiceCollection OnPreConfigureServices(IServiceCollection services)
        {
            // 代办:这里获取配置来源需要替换，这里固化从appsetting.json中获取了
            services.Configure<MbpCachingModuleOptions>(services.BuildServiceProvider().GetService<IConfiguration>().GetSection("Mbp:Caching"));

            return services;
        }

        public override IServiceCollection ConfigureServices(IServiceCollection services)
        {
            // 注册Mbp缓存提供程序
            _cacheProvider = services.AddMbpCache();

            return services;
        }

        public override void OnModuleInitialization(IServiceProvider provider)
        {
            // 模块启动的时候加载
            var cachingOptions = provider.GetService<IOptions<MbpCachingModuleOptions>>()?.Value;
            if (cachingOptions != null && cachingOptions.Enable && _cacheProvider == "redis")
            {
                var cache = provider.GetService<IMbpCache>();
                string cacheStamp = cache.GetOrAdd<string>($"NG_{cachingOptions.Redis.AppName}:{cachingOptions.Redis.Environment}:{cachingOptions.Redis.StampKey}", new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString(), absoluteExpirationRelativeToNow: TimeSpan.FromDays(3600));

                if (string.IsNullOrEmpty(cacheStamp))
                {
                    throw new Exception("分布式缓存Redis的时间戳设置失败！缓存设置失败！");
                }
            }
        }
    }
}
