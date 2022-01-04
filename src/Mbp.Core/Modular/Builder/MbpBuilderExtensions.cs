using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Mbp.Modular.Builder
{
    /// <summary>
    /// IMbpBuilder扩展
    /// </summary>
    public static class MbpBuilderExtensions
    {
        /// <summary>
        /// 添加核心模块
        /// </summary>
        internal static IMbpBuilder AddCoreModule(this IMbpBuilder builder)
        {
            // 代办 在这里可以注册一些第三方服务

            return builder;
        }
    }
}
