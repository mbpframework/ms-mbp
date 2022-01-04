using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using Mbp.Caching;

namespace Mbp.Caching
{
    public static class IServiceCollectionExtensions
    {
        public static string AddMbpCache(this IServiceCollection services)
        {
            var loggerFactory = LoggerFactory.Create(o =>
            {
                o.AddConsole();
            });
            var logger = loggerFactory.CreateLogger("AddMbpCache");

            // 根据配置策略进行缓存服务的初始化
            var cachingOptions = services.BuildServiceProvider().GetService<IOptions<MbpCachingModuleOptions>>()?.Value;
            if (cachingOptions == null)
            {
                logger.LogWarning("底层框架Mbp,加载缓存模块失败，信息：配置读取失败！");
                return string.Empty;
            }
            switch (cachingOptions.Provider)
            {
                case "memory":
                    {
                        return InitMemoryCache(services, cachingOptions);
                    }
                case "redis":
                    {
                        try
                        {
                            return InitDistributeCache(services, cachingOptions);
                        }
                        catch (Exception ex)
                        {
                            logger.LogWarning($"底层框架Mbp,加载缓存模块异常[redis]，切换为内存缓存，信息：" + ex.Message);

                            return InitMemoryCache(services, cachingOptions);
                        }
                    }
                case "auto":
                    {
                        try
                        {
                            return InitDistributeCache(services, cachingOptions);
                        }
                        catch (Exception ex)
                        {
                            logger.LogWarning($"底层框架Mbp,加载缓存模块异常[auto]，切换为内存缓存，信息：" + ex.Message);

                            return InitMemoryCache(services, cachingOptions);
                        }
                    }
                default:
                    return string.Empty;
            }
        }

        private static string InitDistributeCache(IServiceCollection services, MbpCachingModuleOptions cachingOptions)
        {
            var redisConfigOptions = new ConfigurationOptions()
            {
                CommandMap = CommandMap.Create(new HashSet<string>
                                { // EXCLUDE a few commands
                                "INFO", "CONFIG", "CLUSTER",
                                "PING", "ECHO", "CLIENT","UNLINK"
                                }, available: false),
                KeepAlive = cachingOptions.Redis.KeepAlive,
                DefaultVersion = new Version(cachingOptions.Redis.DefaultVersion),
                Password = cachingOptions.Redis.Password
            };
            cachingOptions.Redis.EndPoints.ForEach(hostAndPort =>
            {
                redisConfigOptions.EndPoints.Add(hostAndPort);
            });

            // redis连接不上时候，缓存提供程序改为memory
            if (!ConnectionMultiplexer.Connect(redisConfigOptions).IsConnected)
            {
                return "memory";
            }

            services.AddStackExchangeRedisCache(options =>
            {
                options.ConfigurationOptions = redisConfigOptions;
            });

            // 注册分布式缓存服务,【注意】生命周期必须是单例的，必须重写AddMemoryCache
            services.AddSingleton(typeof(IMbpCache), typeof(MbpDistributedCache));

            return "redis";
        }

        private static string InitMemoryCache(IServiceCollection services, MbpCachingModuleOptions cachingOptions)
        {
            services.AddMemoryCache(options =>
            {
                options.SizeLimit = cachingOptions.Memory.SizeLimit.Value == 0 ? null : cachingOptions.Memory.SizeLimit;// 缓存条目
                options.ExpirationScanFrequency = TimeSpan.FromMinutes(cachingOptions.Memory.ExpirationScanFrequency);// 过期缓存移除间隔
                options.CompactionPercentage = cachingOptions.Memory.CompactionPercentage;// 缓存条目数超过最大限制时候压缩率
            });

            // 注册本地缓存服务,【注意】生命周期必须是单例的，如果需要替换生命周期，必须重写AddMemoryCache
            services.AddSingleton(typeof(IMbpCache), typeof(MbpMemoryCache));

            return "memory";
        }
    }
}
