using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Mbp.Net.Http
{
    internal interface IHttpClientService
    {
        HttpClient GetHttpClient();

        HttpRequestMessage GetHttpRequest(HttpMethod httpMethod, string requestUri, IDictionary<string, string> parameters = null, IDictionary<string, string> heads = null);

        Task<MbpHttpResponse<TResult>> GetResponseMessage<TResult>(HttpResponseMessage response) where TResult : class;

        Task<TResult> GetResponseMessageRaw<TResult>(HttpResponseMessage response) where TResult : class;
    }
}
