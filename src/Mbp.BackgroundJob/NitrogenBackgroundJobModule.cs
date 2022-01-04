using Hangfire;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using WuhanIns.Nitrogen.Modular;

namespace WuhanIns.Nitrogen.BackgroundJob
{
    /// <summary>
    /// 后台作业模块
    /// </summary>
    [DependsOn(typeof(NitrogenModule),
        typeof(NitrogenComponents))]
    public class NitrogenBackgroundJobModule : NitrogenComponents
    {
        public override IServiceCollection OnPreConfigureServices(IServiceCollection services)
        {
            // 代办:这里获取配置来源需要替换，这里固化从appsetting.json中获取了
            services.Configure<BackgroundJobModuleOptions>(services.BuildServiceProvider().GetService<IConfiguration>().GetSection("Ins:BackgroundJob"));

            return services;
        }

        public override IServiceCollection ConfigureServices(IServiceCollection services)
        {
            var backgroundjobOptions = services.BuildServiceProvider().GetService<IOptions<BackgroundJobModuleOptions>>().Value;

            var mongoUrlBuilder = new MongoUrlBuilder(backgroundjobOptions.JobStorage.ConnectionString);
            var mongoClient = new MongoClient(mongoUrlBuilder.ToMongoUrl());

            if (backgroundjobOptions != null && backgroundjobOptions.Enabled)
            {
                services.AddHangfire(configuration => configuration
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseMongoStorage(mongoClient, mongoUrlBuilder.DatabaseName, new MongoStorageOptions
            {
                MigrationOptions = new MongoMigrationOptions
                {
                    MigrationStrategy = new MigrateMongoMigrationStrategy(),
                    BackupStrategy = new CollectionMongoBackupStrategy()
                },
                Prefix = backgroundjobOptions.JobStorage.TablePrefix,
                CheckConnection = true
            }));

                services.AddHangfireServer(serverOptions =>
                {
                    serverOptions.ServerName = backgroundjobOptions.ServerName;
                });


                services.AddSingleton(typeof(INgJob), typeof(NgJob));
            }

            return services;
        }

        public override void OnModuleInitialization(IApplicationBuilder app)
        {
            var config = app.ApplicationServices.GetService<IOptions<BackgroundJobModuleOptions>>().Value;
            if (config != null && config.Enabled)
            {
                // 启动hangfire中间件
                app.UseHangfireServer();
                app.UseHangfireDashboard();
            }
        }
    }
}
