using System;
using System.Collections.Generic;
using System.Text;

namespace Mbp.Caching
{
    /// <summary>
    /// 内存式缓存（本地）配置选项
    /// </summary>
    internal class MemoryOptions
    {
        /// <summary>
        /// 最大条目限制
        /// </summary>
        public long? SizeLimit { get; set; }

        /// <summary>
        /// 过期缓存移除间隔 单位分钟
        /// </summary>
        public int ExpirationScanFrequency { get; set; }

        /// <summary>
        /// 压缩率
        /// </summary>
        public double CompactionPercentage { get; set; }
    }
}
