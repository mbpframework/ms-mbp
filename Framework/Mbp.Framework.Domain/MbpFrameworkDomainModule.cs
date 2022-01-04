using Mbp.Modular;
using Mbp.Framework.Domain.Share;
using System;
using Microsoft.Extensions.DependencyInjection;
using Mbp.DataAccess;

namespace Mbp.Framework.Domain
{
    [DependsOn(typeof(MbpFrameworkDomainShareModule), typeof(MbpDataAccessModule))]
    public class MbpFrameworkDomainModule : MbpDataAccessModule
    {
        public override IServiceCollection ConfigureServices(IServiceCollection services)
        {
            // 注册DefaultDbContext
            base.ConfigureServices<DefaultDbContext>(services);

            // 注册SecondDbContext
            //ConfigureServices<SecondDbContext>(services);

            return services;
        }
    }
}
