using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mbp.Configuration
{
    /// <summary>
    /// 配置加载器，单体下从本地配置中读取，集群下从Apollo中读取
    /// </summary>
    public interface IConfigProvider
    {
        /// <summary>
        /// 加载配置提供程序
        /// </summary>
        /// <param name="builder">IHostBuilder</param>
        /// <param name="args">命令行参数，优先级最高的配置提供方式</param>
        void UseConfigProvider(IHostBuilder builder, string[] args = null);
    }
}
