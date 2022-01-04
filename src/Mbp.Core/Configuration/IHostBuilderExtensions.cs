using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Mbp.Configuration;
using System;

namespace Mbp.Extensions
{
    /// <summary>
    /// IHostBuilder扩展,扩展配置提供程序
    /// </summary>
    public static class IHostBuilderExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IHostBuilder ConfigureMbpConfiguration(this IHostBuilder builder, IConfigurationRoot configuration, string[] args)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            var configProviderName = configuration.GetSection("Mbp:Config:Provider").Value;
            var configProvider = new ConfigProviderFactory().Create(configProviderName);

            if (configProvider == null) throw new NotImplementedException($"未实现配置提供程序：{configProviderName}");

            configProvider.UseConfigProvider(builder, args);

            return builder;
        }
    }
}
