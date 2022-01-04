using Mbp.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mbp.Caching
{
    /// <summary>
    /// 缓存模块选项
    /// </summary>
    internal class MbpCachingModuleOptions
    {
        /// <summary>
        /// 是否启用缓存
        /// </summary>
        public bool Enable { get; set; } = true;

        /// <summary>
        /// 缓存提供程序
        /// </summary>
        public string Provider { get; set; }

        public int AbsoluteExpirationRelativeToNow { get; set; }

        public int SlidingExpiration { get; set; }

        public MemoryOptions Memory { get; set; }

        public MbpRedisOptions Redis { get; set; }
    }
}
