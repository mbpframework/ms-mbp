using Mbp.Modular;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mbp.Configuration
{
    /// <summary>
    /// 整体配置项
    /// </summary>
    public class GlobalSetting
    {
        /// <summary>
        /// 应用程序WebAPI服务侦听端口
        /// </summary>
        public string HttpPort { get; set; }

        /// <summary>
        /// 应用程序gRPC服务侦听端口
        /// </summary>
        public string GrpcPort { get; set; }

        /// <summary>
        /// 环境变量
        /// </summary>
        public string Environment { get; set; }
    }
}
