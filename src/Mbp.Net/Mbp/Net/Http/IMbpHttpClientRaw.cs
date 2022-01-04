using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Mbp.Net.Http
{
    public interface IMbpHttpClientRaw
    {
        TResult Get<TResult>(string serviceBaseUrl, string relativePath, IDictionary<string, string> parameters, IDictionary<string, string> heads = null)
            where TResult : class;


        Task<TResult> GetAsync<TResult>(string serviceBaseUrl, string relativePath, IDictionary<string, string> parameters, IDictionary<string, string> heads = null)
            where TResult : class;

        Task<TResult> GetAsync<TResult>(string url, IDictionary<string, string> parameters, IDictionary<string, string> heads = null)
            where TResult : class;


        TResult Post<TResult, TContent>(string serviceBaseUrl, string relativePath, TContent content, IDictionary<string, string> heads = null)
            where TResult : class
            where TContent : class;


        Task<TResult> PostAsync<TResult, TContent>(string serviceBaseUrl, string relativePath, TContent content, IDictionary<string, string> heads = null)
            where TResult : class
            where TContent : class;

        Task<TResult> PostAsync<TResult, TContent>(string url, TContent content, IDictionary<string, string> heads = null)
            where TResult : class
            where TContent : class;
    }
}
