using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mbp.Modular;
using Mbp.Ddd;
using Mbp.Ddd.Domain.Repository;
using Mbp.DataAccess.EfCore;
using Mbp.DataAccess.Extensions;
using Mbp.DataAccess.EfCore.Interceptors;
using System;
using Oracle.ManagedDataAccess.Client;

namespace Mbp.DataAccess
{
    /// <summary>
    /// 实体对象映射模块
    /// </summary>
    [DependsOn(typeof(MbpModule), typeof(MbpDddModule))]
    public class MbpDataAccessModule : MbpModule
    {
        public override IServiceCollection OnPreConfigureServices(IServiceCollection services)
        {
            // 代办:这里获取配置来源需要替换，这里固化从appsetting.json中获取了
            services.Configure<OrmModuleOptions>(services.BuildServiceProvider().GetService<IConfiguration>().GetSection("Mbp:Orm"));

            return services;
        }

        public override IServiceCollection ConfigureServices(IServiceCollection services)
        {
            // 注册EF CORE泛型仓储
            services.AddScoped(typeof(INgRepository<,,>), typeof(EfCoreRepository<,,>));

            return services;
        }

        protected void ConfigureServices<TDbContext>(IServiceCollection services)
            where TDbContext : MbpDbContext<TDbContext>
        {
            // 获取Orm模块的数据库连接配置
            DbConfig dbConfig = services.ResolveDbConnectionstring<TDbContext>();

            // oracle支持TNS连接方式
            if(dbConfig.DbType== "Oracle")
            {
                // 设置TnsAdmin路径,从环境变量中读取,先从系统变量，再次用户变量，再次当前进程上
                string tnsAdmin = string.Empty;
                if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("TNS_ADMIN", EnvironmentVariableTarget.Machine)))
                {
                    tnsAdmin = Environment.GetEnvironmentVariable("TNS_ADMIN", EnvironmentVariableTarget.Machine);
                }

                if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("TNS_ADMIN", EnvironmentVariableTarget.User)))
                {
                    tnsAdmin = Environment.GetEnvironmentVariable("TNS_ADMIN", EnvironmentVariableTarget.User);
                }

                if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("TNS_ADMIN", EnvironmentVariableTarget.Process)))
                {
                    tnsAdmin = Environment.GetEnvironmentVariable("TNS_ADMIN", EnvironmentVariableTarget.Process);
                }

                if (string.IsNullOrEmpty(tnsAdmin))
                {
                    throw new Exception("Oracle TnsAdmin 未设置，请在环境变量中设置");
                }

                OracleConfiguration.TnsAdmin = tnsAdmin;
            }

            // 设置数据库连接 根据DbContext名字来获取配置节点
            services.AddDbContext<TDbContext>(options =>
            {
                options.UseMbpDb(dbConfig, services.BuildServiceProvider());
                options.AddInterceptors(new MbpQueryCommandInterceptor(services.BuildServiceProvider()), new MbpQueryConnectionInterceptor(services.BuildServiceProvider()));
            });
        }
    }
}
