using WuhanIns.Nitrogen.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace WuhanIns.Nitrogen.BackgroundJob
{
    /// <summary>
    /// 后台作业模块选项
    /// </summary>
    internal class BackgroundJobModuleOptions
    {
        /// <summary>
        /// 是否启用后台作业
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// 延迟时间
        /// </summary>
        public int Delay { get; set; }

        /// <summary>
        /// 持久化存储类型
        /// </summary>
        public JobStorageOptions JobStorage { get; set; }

        /// <summary>
        /// 服务名
        /// </summary>
        public string ServerName { get; set; }
    }
}
