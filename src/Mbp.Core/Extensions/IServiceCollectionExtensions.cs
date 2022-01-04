using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Mbp.Configuration;
using Mbp.Extensions;
using Mbp.Modular;
using Mbp.Modular.Builder;
using Mbp.Modular.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mbp.Core;

namespace Mbp.Internal.Extensions
{
    /// <summary>
    /// IServiceCollection扩展
    /// </summary>
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// 注册Mbp框架服务
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IMbpBuilder AddMbp(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            // 注入程序集查找器
            services.TryAddSingleton<IAssemblyFinder>(new AssemblyFinder());

            // MbpContext注册
            services.TryAddSingleton<IMbpContextAccessor, MbpContextAccessor>();

            // 复用Builder
            IMbpBuilder builder = services.GetSingletonInstanceOrNull<IMbpBuilder>() ?? new MbpBuilder(services);
            services.TryAddSingleton<IMbpBuilder>(builder);

            // 代办 在这里可以注册更多的服务

            return builder;
        }

        /// <summary>
        /// 从IOC中获取单例对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static T GetSingletonInstanceOrNull<T>(this IServiceCollection services)
        {
            return (T)services
                .FirstOrDefault(d => d.ServiceType == typeof(T)
                && d.Lifetime == ServiceLifetime.Singleton
                && d.ImplementationInstance != null)
                ?.ImplementationInstance;
        }
    }
}
