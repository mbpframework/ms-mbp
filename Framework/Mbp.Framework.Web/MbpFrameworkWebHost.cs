using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Mbp.Extensions;
using Serilog;
using Serilog.Events;
using System;
using System.IO;
using Mbp.Logging;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace Mbp.Framework.Web
{
    /// <summary>
    /// Ng主机创建类
    /// </summary>
    public sealed class MbpFrameworkWebHost
    {
        private static IConfigurationRoot _gConfig = new ConfigurationBuilder()
                      .SetBasePath(AppContext.BaseDirectory)
                      .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                      .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
                      .AddEnvironmentVariables()
                      .Build();

        public static int WebHost<T>(string[] args) where T : class
        {

            if (_gConfig.GetSection("Mbp:Environment").Value == "Development")
            {
                Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(_gConfig)
                .Enrich.FromLogContext()
                .WriteTo.Async(a => a.File($"{AppContext.BaseDirectory}Log/.log", rollingInterval: RollingInterval.Day, outputTemplate: "{Timestamp:HH:mm} || {Level} || {SourceContext:l} || {Message} || {Exception} ||end {NewLine}"))
                .WriteTo.Async(a => a.Console())
                .CreateLogger();
            }
            else
            {
                Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(_gConfig)
                .Enrich.FromLogContext()
                .WriteTo.Async(a => a.File($"{AppContext.BaseDirectory}Log/.log", rollingInterval: RollingInterval.Day, outputTemplate: "{Timestamp:HH:mm} || {Level} || {SourceContext:l} || {Message} || {Exception} ||end {NewLine}"))
                .CreateLogger();
            }

            try
            {
                Log.Information("Mbp开始运行......");
                CreateHostBuilder<T>(args).Build().Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex.StackTrace);
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                // 回收日志记录器
                Log.CloseAndFlush();
            }
        }

        /// <summary>
        /// 主机配置方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IHostBuilder CreateHostBuilder<T>(string[] args) where T : class
        {
            // 主机端口
            var port = int.Parse(_gConfig.GetSection("Mbp:HttpPort").Value);

            var gRpcort = int.Parse(_gConfig.GetSection("Mbp:GrpcPort").Value);

            // 运行环境
            var environment = _gConfig.GetSection("Mbp:Environment").Value;

            // 集成Skywalking
            var isOpenAPM = bool.Parse(_gConfig.GetSection("Mbp:IsOpenAPM").Value);
            if (isOpenAPM)
            {
                Environment.SetEnvironmentVariable("ASPNETCORE_HOSTINGSTARTUPASSEMBLIES", "SkyAPM.Agent.AspNetCore");
            }

            // 代办 在这里配置主机，如日志，配置提供程序，侦听端口等
            return Host.CreateDefaultBuilder(args).UseWindowsService()
                .ConfigureMbpConfiguration(_gConfig, args)
                .ConfigureLogging(logging =>
                {
                    // 清理内置日志提供程序
                    logging.ClearProviders();
                    if (environment == "Development")
                    {
                        // 添加控制台日志提供程序，方便本地调试
                        logging.AddSerilog();
                    }
                    // 添加Mbp日志提供程序，将操作，行为日志，系统日志，统一发送到底层平台的日志服务进行存储。
                    logging.AddProvider(new MbpLoggerProvider());
                })
                .UseDefaultServiceProvider((context, options) =>
                {
                    options.ValidateScopes = true;
                })
                   .ConfigureWebHostDefaults(webBuilder =>
                   {
                       webBuilder
                       .UseKestrel((c) =>
                       {
                           // 提供asp.net core web api服务，协议采用Http1
                           c.ListenAnyIP(port, o =>
                           {
                               o.Protocols = HttpProtocols.Http1;
                           });

                           // 提供gRPC服务，协议必须为Http2
                           c.ListenAnyIP(gRpcort, o =>
                           {
                               o.Protocols = HttpProtocols.Http2;
                           });
                       })
                       .UseIISIntegration()
                       .UseIIS()
                       .UseStartup<T>()
                       .UseEnvironment(environment)
                       ;
                   })
                   .UseServiceProviderFactory(new AutofacServiceProviderFactory());
        }
    }
}
