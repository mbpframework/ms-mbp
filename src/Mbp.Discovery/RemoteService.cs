using System;
using System.Collections.Generic;
using System.Text;

namespace Mbp.Discovery
{
    /// <summary>
    /// 远程服务信息类
    /// </summary>
    public class RemoteService
    {
        /// <summary>
        /// 远程服务地址
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// webapi端口
        /// </summary>
        public int HttpPort { get; set; }

        /// <summary>
        /// grpc服务端
        /// </summary>
        public int GrpcPort { get; set; }
    }
}
