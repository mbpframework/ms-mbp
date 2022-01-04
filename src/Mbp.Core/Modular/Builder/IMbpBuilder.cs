using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Mbp.Modular.Builder
{
    /// <summary>
    /// 定义Mbp应用构造器
    /// </summary>
    public interface IMbpBuilder
    {
        /// <summary>
        /// 获取服务集合
        /// </summary>
        IServiceCollection Services { get; }

        /// <summary>
        /// 获取加载的模块集合
        /// </summary>
        List<MbpModule> Modules { get; }

        List<MbpModule> Source { get; }

        IMbpBuilder StartBuild();

        /// <summary>
        /// 添加指定模块
        /// </summary>
        /// <typeparam name="TModule">要添加的模块类型</typeparam>
        IMbpBuilder AddModule<TModule>() where TModule : MbpModule;

        IMbpBuilder AddModule<TModule, TOption>(Action<TOption> action) where TModule : MbpModule;

        /// <summary>
        /// 添加制定模块
        /// </summary>
        /// <returns></returns>
        IMbpBuilder AddModule(object module);

        IServiceCollection AddModule(IServiceCollection services, MbpModule module);
    }
}
