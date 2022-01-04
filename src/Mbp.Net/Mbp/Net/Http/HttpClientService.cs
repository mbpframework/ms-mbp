using Mbp.Core;
using Mbp.Core.User;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Mbp.Net.Http
{
    internal sealed class HttpClientService : IHttpClientService
    {
        private readonly ICurrentUser _currentUser;
        private readonly IMbpContextAccessor _MbpContextAccessor;
        private readonly IHttpClientFactory _httpClientFactory;

        public HttpClientService(ICurrentUser currentUser, IMbpContextAccessor MbpContextAccessor, IHttpClientFactory httpClientFactory)
        {
            _currentUser = currentUser;
            _MbpContextAccessor = MbpContextAccessor;
            _httpClientFactory = httpClientFactory;
        }

        public HttpClient GetHttpClient()
        {
            var httpClient = _httpClientFactory.CreateClient("WebApi");

            if (!string.IsNullOrEmpty(_currentUser.AccessToken))
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_currentUser.AccessToken}");
            }
            // 响应压缩
            httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");

            // http客户端信息
            httpClient.DefaultRequestHeaders.Add("caller-user", Environment.UserName);
            httpClient.DefaultRequestHeaders.Add("caller-machine", Environment.MachineName);
            httpClient.DefaultRequestHeaders.Add("caller-os", Environment.OSVersion.ToString());

            return httpClient;
        }

        public HttpRequestMessage GetHttpRequest(HttpMethod httpMethod, string requestUri, IDictionary<string, string> parameters = null, IDictionary<string, string> heads = null)
        {
            if (parameters != null)
            {
                // 修复: 需要urlencode
                var strParams = string.Join("&", parameters.Select(o => o.Key + "=" + WebUtility.UrlEncode(o.Value)));
                requestUri = string.Concat(requestUri, '?', strParams);
            }

            var request = new HttpRequestMessage(httpMethod, requestUri);

            if (heads != null)
            {
                foreach (var head in heads)
                {
                    request.Headers.Add(head.Key, head.Value);
                }
            }

            return request;
        }

        public async Task<MbpHttpResponse<TResult>> GetResponseMessage<TResult>(HttpResponseMessage response) where TResult : class
        {
            if (response.IsSuccessStatusCode)
            {
                var responseStream = await response.Content.ReadAsStringAsync();

                JsonSerializerSettings settings = new JsonSerializerSettings()
                {
                    
                };

                var result = JsonConvert.DeserializeObject<MbpHttpResponse<TResult>>(responseStream, settings);

                if (result.State != 0 && result.State != 200)
                {
                    throw new InnerServiceErrorException($"Server Inner Error:{result.Message}");
                }

                return result;
            }

            throw new InnerServiceErrorException($"request fail.statusCode:{response.StatusCode}");
        }

        public async Task<TResult> GetResponseMessageRaw<TResult>(HttpResponseMessage response) where TResult : class
        {
            if (response.IsSuccessStatusCode)
            {
                var responseStream = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<TResult>(responseStream);

                return result;
            }

            throw new InnerServiceErrorException($"request fail.statusCode:{response.StatusCode}");
        }
    }
}
