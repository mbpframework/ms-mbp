using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Mbp.Internal.Extensions;
using Mbp.Core;

namespace Mbp.Modular.Builder
{
    /// <summary>
    /// Mbp应用构造器
    /// </summary>
    public class MbpBuilder : IMbpBuilder
    {
        public MbpBuilder(IServiceCollection services)
        {
            Services = services;
            Modules = new List<MbpModule>();
            Source = GetAllModules();
        }

        public IServiceCollection Services { get; }

        /// <summary>
        /// 运行时目录扫描到的模块
        /// </summary>
        public List<MbpModule> Source { get; }

        /// <summary>
        /// 需要加载的模块
        /// </summary>
        public List<MbpModule> Modules { get; private set; }

        /// <summary>
        /// 开始构建
        /// </summary>
        /// <returns></returns>
        public IMbpBuilder StartBuild()
        {
            // 注册基础模块服务
            RegisterModuleService(Modules);

            return this;
        }

        /// <summary>
        /// 往Mbp中挂载模块
        /// </summary>
        /// <typeparam name="TModule"></typeparam>
        /// <returns></returns>
        public IMbpBuilder AddModule<TModule>() where TModule : MbpModule
        {
            // 重复添加处理
            Type type = typeof(TModule);
            if (Modules.Any(m => m.GetType() == type))
            {
                return this;
            }

            // 加入到挂载集合中
            Modules.Add((MbpModule)Activator.CreateInstance(type));

            return this;
        }

        /// <summary>
        /// 往Mbp中挂载模块
        /// </summary>
        /// <typeparam name="TModule"></typeparam>
        /// <typeparam name="TOption"></typeparam>
        /// <param name="action"></param>
        /// <returns></returns>
        public IMbpBuilder AddModule<TModule, TOption>(Action<TOption> action) where TModule : MbpModule
        {
            // 重复添加处理
            Type type = typeof(TModule);
            if (Modules.Any(m => m.GetType() == type))
            {
                return this;
            }

            // 加入到挂载集合中
            Modules.Add((MbpModule)Activator.CreateInstance(type));

            return this;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        public IMbpBuilder AddModule(object module)
        {
            if (module == null)
                throw new ArgumentNullException($"{nameof(module)}加载失败，模块未包含无参构造函数");

            if (module is MbpModule MbpModule)
            {
                // 重复添加处理
                Type type = MbpModule.GetType();
                if (Modules.Any(m => m.GetType() == type))
                {
                    return this;
                }

                // 加入到挂载集合中
                Modules.Add((MbpModule)Activator.CreateInstance(type));

                return this;
            }
            
            throw new ArgumentException($"{nameof(module)}不是NirogenModule块");
        }

        /// <summary>
        /// 添加Mbp模块服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="module"></param>
        /// <returns></returns>
        public IServiceCollection AddModule(IServiceCollection services, MbpModule module)
        {
            Type type = module.GetType();
            Type serviceType = typeof(MbpModule);

            if (type.BaseType?.IsAbstract == false)
            {
                // 移除多重继承的模块
                ServiceDescriptor[] descriptors = services.Where(m =>
                    m.Lifetime == ServiceLifetime.Singleton && m.ServiceType == serviceType
                    && m.ImplementationInstance?.GetType() == type.BaseType).ToArray();
                foreach (var descriptor in descriptors)
                {
                    services.Remove(descriptor);
                }
            }

            if (!services.Any(m => m.Lifetime == ServiceLifetime.Singleton
            && m.ServiceType == serviceType
            && m.ImplementationInstance?.GetType() == type))
            {
                services.AddSingleton(typeof(MbpModule), module);
                module.OnPreConfigureServices(services);
                module.ConfigureServices(services);
            }

            return services;
        }

        /// <summary>
        /// 扫描运行时目录，反射搜索模块
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        private static List<MbpModule> GetAllModules()
        {
            List<Type> MbpModules = new List<Type>();

            // 指定模块抽象类型
            var type = typeof(IMbpModule);

            // 因为所有依赖都通过denpensOn关联好了，所以这里只要获取运行时目录下的所有Mbp.开头的程序集即可。
            var MbpAssemblyFiles = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "Mbp.*.dll");

            foreach (var MbpAssemblyFile in MbpAssemblyFiles)
            {
                MbpModules.AddRange(Assembly.LoadFrom(MbpAssemblyFile).GetTypes().Where(t => type.IsAssignableFrom(t)
                  && !t.IsAbstract && t.Name != "IMbpModule"));
            }

            // 创建模块实例
            return MbpModules.Select(m => (MbpModule)Activator.CreateInstance(m)).ToList();
        }

        /// <summary>
        /// 注册模块服务
        /// </summary>
        /// <param name="MbpModules"></param>
        private void RegisterModuleService(List<MbpModule> MbpModules)
        {
            // 遍历模块，整理依赖关系
            foreach (var module in MbpModules)
            {
                MbpModule[] tmpModules = new MbpModule[Modules.Count];
                MbpModules.CopyTo(tmpModules);

                if (module == null)
                {
                    throw new MbpException($"模块实例无法找到");
                }
                MbpModules.AddIfNotExist(module);

                // 添加依赖模块实例
                Type[] dependTypes = module.FindDependedModuleTypes();
                foreach (var item in dependTypes)
                {
                    module.Dependencies.AddIfNotExist(Source.Find(m => m.GetType() == item));
                }

                foreach (Type dependType in dependTypes)
                {
                    // 在Mbp中,MbpModule在查找时候过滤掉，查找的模块都是具体的实现模块。
                    if (dependType.IsAbstract) continue;

                    MbpModule dependModule = Source.Find(m => m.GetType() == dependType);
                    if (dependModule == null)
                    {
                        throw new MbpException($"加载模块{module.GetType().FullName}时无法找到依赖模块{dependType.FullName}");
                    }
                }
            }

            // 根据模块依赖关系进行拓扑排序
            MbpModules = MbpModules.SortByDependencies(m => m.Dependencies, new ModuleEqualityComparer());

            foreach (var module in MbpModules)
            {
                AddModule(Services, module);
            }
        }
    }
}
