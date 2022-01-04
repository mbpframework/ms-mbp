using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Mbp.Net.Http
{
    /// <summary>
    /// http请求客户端，可请求其他Mbp微服务，不能用于服务发现
    /// </summary>
    public interface IMbpHttpClient
    {

        MbpHttpResponse<TResult> Get<TResult>(string serviceBaseUrl, string relativePath, IDictionary<string, string> parameters, IDictionary<string, string> heads = null)
            where TResult : class;


        Task<MbpHttpResponse<TResult>> GetAsync<TResult>(string serviceBaseUrl, string relativePath, IDictionary<string, string> parameters, IDictionary<string, string> heads = null)
            where TResult : class;

        Task<MbpHttpResponse<TResult>> GetAsync<TResult>(string url, IDictionary<string, string> parameters, IDictionary<string, string> heads = null)
            where TResult : class;


        MbpHttpResponse<TResult> Post<TResult, TContent>(string serviceBaseUrl, string relativePath, TContent content, IDictionary<string, string> heads = null)
            where TResult : class
            where TContent : class;


        Task<MbpHttpResponse<TResult>> PostAsync<TResult, TContent>(string serviceBaseUrl, string relativePath, TContent content, IDictionary<string, string> heads = null)
            where TResult : class
            where TContent : class;

        Task<MbpHttpResponse<TResult>> PostAsync<TResult, TContent>(string url, TContent content, IDictionary<string, string> heads = null)
            where TResult : class
            where TContent : class;
    }
}
