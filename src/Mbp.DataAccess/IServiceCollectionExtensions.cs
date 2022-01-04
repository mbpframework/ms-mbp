using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Mbp.DataAccess.Extensions
{
    /// <summary>
    /// IServiceCollectionExtensions
    /// </summary>
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// 根据DbContext获取数据库连接配置信息
        /// </summary>
        /// <typeparam name="TDbContext"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static DbConfig ResolveDbConnectionstring<TDbContext>(this IServiceCollection services)
        {
            return services.BuildServiceProvider().GetService<IOptions<OrmModuleOptions>>().Value
                ?.DbConnections[typeof(TDbContext).Name];
        }
    }
}
