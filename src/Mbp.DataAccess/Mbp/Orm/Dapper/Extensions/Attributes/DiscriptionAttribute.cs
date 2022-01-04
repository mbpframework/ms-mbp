using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WuhanIns.Nitrogen.Orm.Dapper.Extensions.Attributes
{
    /// <summary>
    /// 描述特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property| AttributeTargets.Class| AttributeTargets.Method)]
    public class DiscriptionAttribute : Attribute
    {
        /// <summary>
        /// 描述
        /// </summary>
        public string discription { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        /// <param name="discription"></param>
        public DiscriptionAttribute(string discription)
        {
            this.discription = discription;
        }
    }
}
