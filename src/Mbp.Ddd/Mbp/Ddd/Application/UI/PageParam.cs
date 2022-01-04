using System;
using System.Collections.Generic;
using System.Text;

namespace Mbp.Ddd.Application.UI
{
    public class PageParam
    {
        /// <summary>
        /// 页码，起始为0
        /// </summary>
        public int PageIndex { get; set; }
        /// <summary>
        /// 每页数据量
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 默认为null，如果非null则返回结果不计算总数
        /// </summary>
        public int? TotalRows { get; set; }
    }
}
