
namespace Mbp.Net.Http
{
    public class MbpHttpResponse<TResult>
    {
        /// <summary>
        /// 返回的数据
        /// </summary>
        public TResult Content { get; set; }

        /// <summary>
        /// 状态码
        /// </summary>
        public int State { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string Message { get; set; }
    }
}
