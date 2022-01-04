using Mbp.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;

namespace Microsoft.AspNetCore.Hosting
{
    /// <summary>
    /// 扩展IWebHostBuilder，配置日志提供程序
    /// </summary>
    public static class IWebHostBuilderExtensions
    {
        public static IWebHostBuilder UseMbpLog(this IWebHostBuilder builder, IConfigurationRoot configuration)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            var loggerProviderName = configuration.GetSection("Mbp:Logger:Provider").Value;

            var loggerProvider = new LoggerProviderFactory().Create(loggerProviderName);
            if (loggerProvider == null) throw new NotImplementedException($"未实现日志提供程序：{loggerProviderName}");

            loggerProvider.UseLog(builder, configuration);

            return builder;
        }
    }
}
