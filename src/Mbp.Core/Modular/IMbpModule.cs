using Microsoft.Extensions.DependencyInjection;
using System;

namespace Mbp.Modular
{
    /// <summary>
    /// 抽象模块的接口，留作公共行为扩展用
    /// </summary>
    public interface IMbpModule
    {

        IServiceCollection OnPreConfigureServices(IServiceCollection services);

        IServiceCollection ConfigureServices(IServiceCollection services);

        void OnModuleInitialization(IServiceProvider provider);

        void OnPreModuleInitialization(IServiceProvider provider);
    }
}
