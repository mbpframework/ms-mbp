using System;
using System.Collections.Generic;
using System.Text;

namespace Mbp.Ddd.Application.UI
{
    public class OrderByParam
    {
        public OrderByParam()
        {
            this.Order = OrderBy.ASC;
        }
        /// <summary>
        /// 字段名
        /// </summary>
        public string Field { get; set; }

        /// <summary>
        /// 排序方式:ASC/DESC，默认ASC
        /// </summary>
        public OrderBy Order { get; set; }

        public enum OrderBy
        {
            ASC = 0,
            DESC = 1
        }
    }
}
