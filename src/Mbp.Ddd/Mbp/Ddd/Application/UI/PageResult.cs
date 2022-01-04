using System;
using System.Collections.Generic;
using System.Text;

namespace Mbp.Ddd.Application.UI
{
    public class PageResult<T>
    {
        /// <summary>
        /// 数据
        /// </summary>
        public List<T> Data { get; set; }

        /// <summary>
        /// 总数
        /// </summary>
        public int TotalRows { get; set; }
    }
}
