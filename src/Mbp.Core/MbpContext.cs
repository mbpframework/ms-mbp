using System.Collections.Generic;

namespace Mbp.Core
{
    /// <summary>
    /// Mbp上下文,默认实现
    /// </summary>
    public sealed class MbpContext
    {
        /// <summary>
        /// 获取或设置Mbp上下文信息存储
        /// </summary>
        public Dictionary<object, object> Items { get; set; }

        /// <summary>
        /// 获取或设置跟踪请求的唯一标识
        /// </summary>
        public string TraceIdentifier { get; set; }
    }
}
