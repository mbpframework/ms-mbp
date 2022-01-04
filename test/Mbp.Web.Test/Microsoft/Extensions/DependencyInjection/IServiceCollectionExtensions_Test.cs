using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Shouldly;
using Mbp.Modular.Builder;
using Mbp.Extensions;
using Mbp.Internal.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public class IServiceCollectionExtensions_Test
    {
        [Fact]
        public void AddMbp_Test()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddMbp().ShouldNotBeNull();
        }

        [Fact]
        public void GetSingletonInstanceOrNull_Test()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddSingleton<AddSingleton_Test>();

            services.GetSingletonInstanceOrNull<AddSingleton_Test>().ShouldBeNull();

            services.AddSingleton(new AddSingleton_Test());

            services.GetSingletonInstanceOrNull<AddSingleton_Test>().ShouldNotBeNull();
        }
    }

    public class AddSingleton_Test
    {
        
    }
}
