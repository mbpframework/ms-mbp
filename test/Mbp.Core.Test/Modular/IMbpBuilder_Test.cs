using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Shouldly;
using Mbp.Test.Base;
using Mbp.Modular.Builder;
using Microsoft.Extensions.DependencyInjection;
using Mbp.Modular;

namespace Mbp.Core.Test.Modular
{
    public class IMbpBuilder_Test : MbpTestBase
    {
        private readonly IMbpBuilder _MbpBuilder;

        public IMbpBuilder_Test()
        {
            _MbpBuilder = provider.GetService<IMbpBuilder>();
            services.AddScoped(typeof(IMbpService_Test), typeof(MbpService_Test));
        }

        [Fact]
        public void AddModule_Test()
        {
            _MbpBuilder.AddModule<MbpTestModule_A>();

            _MbpBuilder.Modules.Count.ShouldBe(1);
        }

        [Fact]
        public void AddModule_TwoArgs_Test()
        {
            _MbpBuilder.AddModule(services, new MbpTestModule_A());
            services.BuildServiceProvider().GetService<IMbpService_Test>().GetName().ShouldBe("Mbp Service Inject Test");
        }

        [Fact]
        public void StartBuild_Test()
        {
            _MbpBuilder.AddModule<MbpTestModule_A>();

            _MbpBuilder.StartBuild();

            services.BuildServiceProvider().GetService<IMbpService_Test>().GetName().ShouldBe("Mbp Service Inject Test");
        }
    }

    public class MbpTestModule_A : MbpModule
    {
        public override EnumModuleLevel Level => base.Level;

        public override string ModuleName => base.ModuleName;

        public override IServiceCollection ConfigureServices(IServiceCollection services)
        {
            services.AddScoped(typeof(IMbpService_Test), typeof(MbpService_Test));

            return services;
        }
    }
}
