using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Mbp.Modular.Builder;
using System;
using Xunit;
using Mbp.Extensions;
using Mbp.Internal.Extensions;

namespace Mbp.Test.Base
{
    public class MbpTestBase
    {
        protected IServiceProvider provider;

        protected ServiceCollection services;

        public MbpTestBase()
        {
            services = new ServiceCollection();

            IMbpBuilder builder = services.GetSingletonInstanceOrNull<IMbpBuilder>() ?? new MbpBuilder(services);
            services.TryAddSingleton<IMbpBuilder>(builder);

            provider = services.BuildServiceProvider();
        }
    }
}
