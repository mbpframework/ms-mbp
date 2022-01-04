using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mbp.Ddd.Application.UI
{
    public class PagedList<T>
    {
        /// <summary>
        /// 获取或设置总记录数。
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// 获取或设置页面大小。
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 获取或设置页码。
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 获取或设置当前页面的数据。
        /// </summary>
        public List<T> Content { get; set; }

        /// <summary>
        /// 获取或设置总页数。
        /// </summary>
        public int Pagecount { get; set; }

        /// <summary>
        /// 书签
        /// </summary>
        public int BookMark { get; set; }

        /// <summary>
        /// 初始化一个新的<c>PagedResult{T}</c>类型的实例。
        /// </summary>
        public PagedList()
        {
            Content = new List<T>();
        }

        /// <summary>
        /// 初始化一个新的<c>PagedResult{T}</c>类型的实例。
        /// </summary>
        /// <param name="total">总记录数。</param>
        /// <param name="pagecount">页数。</param>
        /// <param name="pageSize">页面大小。</param>
        /// <param name="pageIndex">页码。</param>
        /// <param name="data">当前页面的数据。</param>
        /// <param name="bookMark">书签</param>
        public PagedList(int total, int pagecount, int pageSize, int pageIndex, List<T> data, int bookMark = 0)
        {
            this.Pagecount = pagecount;
            this.Total = total;
            this.PageSize = pageSize;
            this.PageIndex = pageIndex;
            this.Content = data;
            BookMark = bookMark;
        }
    }
}
