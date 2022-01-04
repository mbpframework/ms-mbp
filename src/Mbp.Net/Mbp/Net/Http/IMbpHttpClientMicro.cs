using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Mbp.Net.Http
{
    /// <summary>
    /// http请求客户端，请求Mbp微服务，可使用服务发现功能
    /// </summary>
    public interface IMbpHttpClientMicro
    {
        /// <summary>
        /// 请求服务，Get方式
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="serviceName"></param>
        /// <param name="parameters"></param>
        /// <param name="heads"></param>
        /// <returns></returns>
        MbpHttpResponse<TResult> Get<TResult>(string serviceName, string relativePath, IDictionary<string, string> parameters, IDictionary<string, string> heads = null)
            where TResult : class;

        /// <summary>
        /// 请求服务，Get方式
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="serviceName"></param>
        /// <param name="parameters"></param>
        /// <param name="heads"></param>
        /// <returns></returns>
        Task<MbpHttpResponse<TResult>> GetAsync<TResult>(string serviceName, string relativePath, IDictionary<string, string> parameters, IDictionary<string, string> heads = null)
            where TResult : class;

        /// <summary>
        /// 请求服务，Post方式
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <typeparam name="TContent"></typeparam>
        /// <param name="serviceName"></param>
        /// <param name="content"></param>
        /// <param name="heads"></param>
        /// <returns></returns>
        MbpHttpResponse<TResult> Post<TResult, TContent>(string serviceName, string relativePath, TContent content, IDictionary<string, string> heads = null)
            where TResult : class
            where TContent : class;

        /// <summary>
        /// 请求服务，Post方式
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <typeparam name="TContent"></typeparam>
        /// <param name="serviceName"></param>
        /// <param name="content"></param>
        /// <param name="heads"></param>
        /// <returns></returns>
        Task<MbpHttpResponse<TResult>> PostAsync<TResult, TContent>(string serviceName, string relativePath, TContent content, IDictionary<string, string> heads = null)
            where TResult : class
            where TContent : class;
    }
}
