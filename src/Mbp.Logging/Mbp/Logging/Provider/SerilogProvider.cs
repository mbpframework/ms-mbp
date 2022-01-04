using Exceptionless;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using System;
using System.IO;

namespace Mbp.Logging
{
    /// <summary>
    /// Serilog提供程序
    /// </summary>
    internal class SerilogProvider : IMbpLoggerProvider
    {
        public void UseLog(IWebHostBuilder builder, IConfigurationRoot configuration)
        {
            // asp.net core集成Serilog
            if (bool.Parse(configuration.GetSection("Mbp:Logger:IsWriteToExceptionless").Value))
            {
                // Exceptionless客户端
                ExceptionlessClient client = new ExceptionlessClient(c =>
                {
                    c.ApiKey = configuration.GetSection("Mbp:Logger:ApiKey").Value;
                    c.ServerUrl = configuration.GetSection("Mbp:Logger:ServerUrl").Value;
                });

                var config = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                   .AddJsonFile($"appsettings.Development.json",
                                     optional: true, reloadOnChange: true)
                   .AddEnvironmentVariables()
                   .Build();

                builder.UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration.ReadFrom.Configuration(config)
                           .WriteTo.Console()
                           .WriteTo.File($"{AppContext.BaseDirectory}Log/.log", rollingInterval: RollingInterval.Day, outputTemplate: "{Timestamp:HH:mm} || {Level} || {SourceContext:l} || {Message} || {Exception} ||end {NewLine}")
                           .WriteTo.Exceptionless(null, true, LogEventLevel.Information, client)
                           );
            }
            else
            {
                var config = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                   .AddJsonFile($"appsettings.Development.json",
                                     optional: true, reloadOnChange: true)
                   .AddEnvironmentVariables()
                   .Build();

                builder.UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration.ReadFrom.Configuration(config)
                           .WriteTo.Console()
                           .WriteTo.File($"{AppContext.BaseDirectory}Log/.log", rollingInterval: RollingInterval.Day, outputTemplate: "{Timestamp:HH:mm} || {Level} || {SourceContext:l} || {Message} || {Exception} ||end {NewLine}")
                           );
            }
        }
    }
}
