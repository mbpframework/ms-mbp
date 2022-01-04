using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mbp.Logging
{
    /// <summary>
    /// 日志提供程序功能接口
    /// </summary>
    public interface IMbpLoggerProvider
    {
        void UseLog(IWebHostBuilder builder, IConfigurationRoot configuration);
    }
}
