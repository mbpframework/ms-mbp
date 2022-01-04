using System;
using System.Collections.Generic;
using System.Text;

namespace Mbp.Caching
{
    /// <summary>
    /// 分布式缓存，Redis配置选项
    /// </summary>
    internal class MbpRedisOptions
    {

        public string AppName { get; set; }

        public string Environment { get; set; }

        public string StampKey { get; set; }

        public List<string> EndPoints { get; set; }

        public string Password { get; set; }

        public int ConnectTimeout { get; set; }

        public string ClientName { get; set; }

        public int KeepAlive { get; set; }

        public string DefaultVersion { get; set; }
    }
}
