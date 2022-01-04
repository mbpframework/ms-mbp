using Microsoft.Extensions.Hosting;
using Mbp.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Mbp.Config.Apollo
{
    internal class ApolloProvider : IConfigProvider
    {
        public void UseConfigProvider(IHostBuilder builder, string[] args = null)
        {
            // 获取配置 代办 需要获取配置中心的配置
            var gconfig = new ConfigurationBuilder()
                   .SetBasePath(AppContext.BaseDirectory)
                   .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                   .AddEnvironmentVariables()
                   .Build();

            builder.ConfigureAppConfiguration((context, config) =>
            {
                // 清理所有配置提供源
                config.Sources.Clear();

                // 添加apollo配置
                config.AddApollo(gconfig.GetSection("Mbp:Config:Apollo"));

                // 添加环境变量配置
                config.AddEnvironmentVariables();

                // 添加命令行配置
                if (args != null)
                {
                    config.AddCommandLine(args);
                }
            });
        }
    }
}
