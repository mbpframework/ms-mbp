using Mbp.DataAccess;
using Microsoft.Extensions.DependencyInjection;

namespace Res.DataAccess
{
    public class InsSampleDataAccessModule : MbpDataAccessModule
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
