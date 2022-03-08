using Microsoft.Extensions.DependencyInjection;
using WuhanIns.Nitrogen.Framework.DataAccess;

namespace Ins.Sample.DataAccess
{
    public class InsDemoDataAccessModule : NgFrameworkDataAccessModule
    {
        public override IServiceCollection ConfigureServices(IServiceCollection services)
        {
            // 注册DefaultDbContext
            ConfigureServices<DefaultDbContext>(services);

            // 注册SecondDbContext
            //ConfigureServices<SecondDbContext>(services);

            return services;
        }
    }
}
