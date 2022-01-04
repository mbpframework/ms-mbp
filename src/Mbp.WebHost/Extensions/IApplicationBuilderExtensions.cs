using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mbp.Modular;
using Mbp.Modular.Builder;
using Mbp.Modular.Reflection;
using Mbp.AspNetCore;
using System;
using System.Diagnostics;
using Mbp.Internal.Extensions;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using Mbp.Net.gRPC;
using System.Linq;
using Microsoft.AspNetCore.Routing;

namespace Mbp.WebHost
{
    /// <summary>
    /// 应用构造器扩展类
    /// </summary>
    public static class IApplicationBuilderExtensions
    {
        // gRPC服务端
        private static readonly MethodInfo MapGrpcServiceMethodInfo = typeof(IApplicationBuilderExtensions).GetMethod(nameof(MapGrpcService), BindingFlags.Static | BindingFlags.NonPublic);

        private static void MapGrpcService<T>(IEndpointRouteBuilder endpoint) where T : class
        {
            endpoint.MapGrpcService<T>();
        }

        /// <summary>
        /// 将基础设施的模块加入到asp.net core管道
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseMbp(this IApplicationBuilder app)
        {
            // 获取依赖服务的提供程序
            IServiceProvider provider = app.ApplicationServices;
            ILoggerFactory factory = provider.GetService<ILoggerFactory>();
            IMbpBuilder builder = provider.GetService<IMbpBuilder>();

            ILogger logger = factory.CreateLogger("ApplicationBuilderExtensions");
            logger.LogInformation(0, "Mbp框架初始化开始");
            Stopwatch watch = Stopwatch.StartNew();

            foreach (MbpModule module in builder.Modules)
            {
                string moduleName = module.GetType().GetDescription();
                logger.LogInformation($"正在初始化模块 “{moduleName}”");
                module.OnPreModuleInitialization(provider);
                if (module is MbpAspNetModule componentModule)
                {
                    componentModule.OnModuleInitialization(app);
                }
                else
                {
                    module.OnModuleInitialization(provider);
                }
                logger.LogInformation($"模块 “{moduleName}” 初始化完成");
            }

            watch.Stop();
            logger.LogInformation(0, $"Mbp框架初始化完成，耗时：{watch.ElapsedMilliseconds} 毫秒");
            return app;
        }

        /// <summary>
        /// 支持gRPC协议
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseGrpcService(this IApplicationBuilder app)
        {
            // 运行时目录
            var runtimePath = AppContext.BaseDirectory;

            // 获取扫描目录
            var directories = Directory.GetDirectories(runtimePath, "Mbp.*");

            // 扫描Mbp.*的文件目录，挂载业务系统
            foreach (var directoryPath in directories)
            {
                var systemName = Path.GetFileName(directoryPath);
                var appServiceFiles = Directory.GetFiles(directoryPath, $"{systemName}.Application.dll");
                foreach (var appService in appServiceFiles)
                {
                    var applicatonAssembly = Assembly.LoadFrom(Path.Combine(directoryPath, appService));
                    Type tNgGrpcService = typeof(IMbpGrpcService);

                    var gRpcServices = applicatonAssembly.GetTypes().Where(s => tNgGrpcService.IsAssignableFrom(s));

                    app.UseEndpoints(endpoints =>
                    {
                        foreach (var gRpcServerType in gRpcServices)
                        {
                            MapGrpcServiceMethodInfo
                           .MakeGenericMethod(gRpcServerType)
                           .Invoke(null, new object[] { endpoints });
                        }
                    });
                }
            }

            return app;
        }
    }
}
