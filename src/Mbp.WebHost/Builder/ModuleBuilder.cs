using Autofac;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Mbp.Configuration;
using Mbp.Modular;
using Mbp.Modular.Builder;
using Mbp.AspNetCore.Api.Filters;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Mbp.Internal.Extensions;
using Mbp.AspNetCore;
using Mbp.WebHost.Filter;

namespace Mbp.WebHost.Builder
{
    /// <summary>
    /// 模块构造器
    /// </summary>
    public sealed class ModuleBuilder : IModuleBuidler
    {
        private IServiceCollection _services = null;

        /// <summary>
        /// 单例实例
        /// </summary>
        public static ModuleBuilder Instance { get; } = new ModuleBuilder();

        static ModuleBuilder()
        {
        }

        /// <summary>
        /// 初始化构造器
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public ModuleBuilder Init(IServiceCollection services)
        {
            _services = services;
            return this;
        }

        /// <summary>
        /// 构建业务开发脚手架
        /// </summary>
        /// <returns></returns>
        public IModuleBuidler BuildApplicationFalsework()
        {
            // 运行时目录
            var runtimePath = AppContext.BaseDirectory;

            // 获取扫描目录
            var directories = Directory.GetDirectories(runtimePath, "Mbp.*");

            // 获取业务基本配置
            var configurations = _services.BuildServiceProvider().GetService<IOptions<ProductSetting>>().Value;

            // Web模块配置
            var webOptions = _services.BuildServiceProvider().GetService<IOptions<WebModuleOptions>>().Value;

            // 扫描调试模式下的挂载目录
            if (webOptions.ApplicationModulePath?.Count > 0)
            {
                foreach (var modulePath in webOptions.ApplicationModulePath)
                {
                    ProductBaseConfig currentSysConfig = GetCurrentConfig(configurations, modulePath.Key);
                    Log.Information($"===>[调试模式]开始载入业务系统 “{modulePath.Key}” ");
                    Stopwatch watch = Stopwatch.StartNew();
                    LoadProductModule(modulePath.Value, modulePath.Key, currentSysConfig);

                    watch.Stop();
                    Log.Information($"<===[调试模式]业务系统 “{modulePath.Key}” 载入完成，耗时：{watch.ElapsedMilliseconds} 毫秒");
                }
            }

            // 扫描Mbp.*的文件目录，挂载业务系统
            foreach (var directoryPath in directories)
            {
                var systemName = Path.GetFileName(directoryPath);
                ProductBaseConfig currentSysConfig = GetCurrentConfig(configurations, systemName);
                Log.Information($"===>开始载入业务系统 “{systemName}” ");
                Stopwatch watch = Stopwatch.StartNew();
                LoadProductModule(directoryPath, systemName, currentSysConfig);

                watch.Stop();
                Log.Information($"<===业务系统 “{systemName}” 载入完成，耗时：{watch.ElapsedMilliseconds} 毫秒");
            }

            // 加入运行时过滤器
            _services.AddControllers().AddMvcOptions(options =>
            {
                if (string.IsNullOrEmpty(webOptions.IdentityServer))
                {
                    options.Filters.Add(typeof(CustomizeAuthorizeFilter));
                    Log.Information($"加入运行时过滤器【{typeof(CustomizeAuthorizeFilter).FullName}】");
                }
            });

            return this;
        }

        private static ProductBaseConfig GetCurrentConfig(ProductSetting? configurations, string systemName)
        {
            ProductBaseConfig currentSysConfig = null;
            if ((configurations?.ProductConfigs).ContainsKey(systemName))
            {
                currentSysConfig = configurations?.ProductConfigs[systemName];
            }
            else
            {
                Log.Warning($"无法找到业务模块[{systemName}]相关的配置，请确保配置是否完善！");
            }

            return currentSysConfig;
        }

        // 加载业务应用模块
        private void LoadProductModule(string directoryPath, string systemName, ProductBaseConfig? currentSysConfig)
        {
            if (!Directory.Exists(directoryPath))
            {
                Log.Warning($"[{systemName}]模块文件未找到，不存在路径[{directoryPath}]，跳过当前模块加载......");
                return;
            }

            var appServiceFiles = Directory.GetFiles(directoryPath, $"{systemName}.Application.dll")
                                .Union(Directory.GetFiles(directoryPath, $"{systemName}.Service.dll")).ToArray();

            Log.Information($"找到【{appServiceFiles.Length}】个应用程序部件文件，装载中......");
            foreach (var appService in appServiceFiles)
            {
                var applicatonAssembly = Assembly.LoadFrom(Path.Combine(directoryPath, appService));

                // 手动删除 Mbp平台dll 阻止依赖资源加载 代办 考虑代码方式阻止依赖加载
                _services.AddControllers().AddApplicationPart(applicatonAssembly).AddControllersAsServices();

                Type tFilter = typeof(MbpFilter);

                // 反射查找所有业务开发的过滤器，根据业务开发配置的过滤器进行注册
                var filters = applicatonAssembly.GetTypes().Where(s => tFilter.IsAssignableFrom(s));
                if (currentSysConfig?.Filters != null)
                {
                    foreach (var item in currentSysConfig.Filters.Where(f => f.StartsWith(systemName)))
                    {
                        if (filters.Any(f => f.FullName == item))
                        {
                            Log.Information($"找到业务系统过滤器【{filters.Where(f => f.FullName == item).First().FullName}】");
                            _services.AddControllers().AddMvcOptions(options =>
                            {
                                options.Filters.Add(filters.Where(f => f.FullName == item).First());
                                Log.Information($"加入业务系统过滤器【{filters.Where(f => f.FullName == item).First().FullName}】");
                            });
                        }
                    }
                }
            }

            var bussinessModuleFiles = Directory.GetFiles(directoryPath, $"{systemName}.*.dll");
            // 指定模块抽象类型
            var type = typeof(IMbpModule);
            List<Type> MbpModules = new List<Type>();
            foreach (var bussinessModuleFile in bussinessModuleFiles)
            {
                MbpModules.AddRange(Assembly.LoadFrom(bussinessModuleFile).GetTypes().Where(t => type.IsAssignableFrom(t)
              && !t.IsAbstract && t.Name != "IMbpModule"));
            }
            // 实例化业务模块
            var bussinessModules = MbpModules.Select(m => (MbpModule)Activator.CreateInstance(m)).ToList();

            // 注册模块服务
            RegisterModuleService(bussinessModules, m => true);
        }

        /// <summary>
        /// 构建Mbp脚手架
        /// </summary>
        /// <returns></returns>
        public IModuleBuidler BuildMbpFalsework()
        {
            var MbpBuilder = _services.GetSingletonInstanceOrNull<IMbpBuilder>();
            var sources = MbpBuilder.Source;
            var ngFrameworkModules = new List<MbpModule>();
            // 注册Mbp脚手架服务
            foreach (MbpModule frameworkModule in sources.Where(s => s.ModuleName.StartsWith("MbpFramework")))
            {
                ngFrameworkModules.Add((MbpModule)Activator.CreateInstance(frameworkModule.GetType()));
            }

            // 注册模块服务
            RegisterModuleService(ngFrameworkModules, m => m.Name.StartsWith("MbpFramework"));

            return this;
        }

        /// <summary>
        /// 注册模块服务
        /// </summary>
        /// <param name="MbpModules"></param>
        /// <param name="where">依赖查找过滤条件</param>
        private void RegisterModuleService(List<MbpModule> MbpModules, Func<Type, bool> where)
        {
            // 链接依赖模块
            foreach (var bussinessModule in MbpModules)
            {
                var dependTypes = bussinessModule.FindDependedModuleTypes().Where(where);
                foreach (var item in dependTypes)
                {
                    bussinessModule.Dependencies.AddIfNotExist(MbpModules.Find(m => m.GetType() == item));
                }
            }

            // 拓扑排序
            MbpModules = MbpModules.SortByDependencies(m => m.Dependencies, new ModuleEqualityComparer());

            // 注册NgFramework模块服务
            foreach (var module in MbpModules)
            {
                Log.Information($"开始注册模块  “{module.ModuleName}” ");

                _services.GetSingletonInstanceOrNull<IMbpBuilder>().AddModule(_services, module);

                Log.Information($"模块  “{module.ModuleName}” 注册完成");
            }
        }
    }
}
