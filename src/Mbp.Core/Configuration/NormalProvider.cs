using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Mbp.Configuration
{
    internal class NormalProvider : IConfigProvider
    {
        public void UseConfigProvider(IHostBuilder builder, string[] args = null)
        {
            builder.ConfigureAppConfiguration((hostingContext, config) =>
            {
                // 移除所有配置提供程序
                config.Sources.Clear();

                var env = hostingContext.HostingEnvironment;

                config.SetBasePath(AppContext.BaseDirectory);

                // 设置Mbp的配置数据文件
                // 平台及产品配置
                config.AddJsonFile("appsettings_normal.json", optional: true, reloadOnChange: true)
                      .AddJsonFile($"appsettings_normal.{env.EnvironmentName}.json",
                                     optional: true, reloadOnChange: true);

                // 二开配置，二开的配置文件会覆盖平台及产品的同名配置
                config.AddJsonFile("appsettings_normal.customize.json", optional: true, reloadOnChange: true)
                      .AddJsonFile($"appsettings_normal.customize.{env.EnvironmentName}.json",
                                     optional: true, reloadOnChange: true);

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
