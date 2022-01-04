using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Mbp.Framework.DataAccess.Interceptors;
using Mbp.Modular;
using Mbp.DataAccess;
using Mbp.DataAccess.Extensions;
using Mbp.AspNetCore;

namespace Mbp.Framework.DataAccess
{
    [DependsOn(typeof(MbpOrmModule))]
    public class MbpFrameworkDataAccessModule : MbpModule
    {
        public override IServiceCollection ConfigureServices(IServiceCollection services)
        {
            return services;
        }

        protected void ConfigureServices<TDbContext>(IServiceCollection services)
            where TDbContext : MbpDbContext<TDbContext>
        {
            // 获取Orm模块的数据库连接配置
            DbConfig dbConfig = services.ResolveDbConnectionstring<TDbContext>();

            // 设置数据库连接 根据DbContext名字来获取配置节点
            services.AddDbContext<TDbContext>(options =>
            {
                options.UseMbpDb(dbConfig, services.BuildServiceProvider());
                options.AddInterceptors(new MbpQueryCommandInterceptor(services.BuildServiceProvider()), new MbpQueryConnectionInterceptor(services.BuildServiceProvider()));
            });
        }
    }
}
