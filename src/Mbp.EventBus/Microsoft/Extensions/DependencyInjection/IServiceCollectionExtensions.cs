using Mbp.EventBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Savorboard.CAP.InMemoryMessageQueue;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        public static void AddMbpEventBus(this IServiceCollection services)
        {
            // 根据配置策略进行缓存服务的初始化
            var cachingOptions = services.BuildServiceProvider().GetService<IOptions<EventBusModuleOptions>>()?.Value;
            if (cachingOptions == null) throw new ArgumentNullException("配置参数不正确");

            services.AddCap(x =>
            {
                switch (cachingOptions.MessageQueue.Provider)
                {
                    case "memory":
                        {
                            x.UseInMemoryMessageQueue();
                        }
                        break;
                    case "rabbitMQ ":
                        {
                            x.UseRabbitMQ(cachingOptions.MessageQueue.HostName);
                        }
                        break;
                }

                switch (cachingOptions.EventLogStorage.Provider)
                {
                    case "memory":
                        {
                            x.UseInMemoryStorage();
                        }
                        break;
                    case "MySql":
                        {
                            x.UseMySql(o =>
                            {
                                o.ConnectionString = cachingOptions.EventLogStorage.ConnectionString;
                                o.TableNamePrefix = cachingOptions.EventLogStorage.TableNamePrefix;
                            });
                        }
                        break;
                    case "MongoDB":
                        {
                            x.UseMongoDB(o =>
                            {
                                o.DatabaseConnection = "";
                                o.DatabaseName = "";
                                o.PublishedCollection = "";
                                o.ReceivedCollection = "";
                            });
                        }
                        break;
                }
            });
        }
    }
}
