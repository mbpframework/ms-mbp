using Mbp.Discovery;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace Mbp.Net.Http
{
    internal class MbpHttpClientMicro : IMbpHttpClientMicro
    {
        private readonly IMbpDiscovery _ngDiscovery;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly IHttpClientService _httpClientService;

        public MbpHttpClientMicro(IMbpDiscovery ngDiscovery, IHttpClientService httpClientService)
        {
            _ngDiscovery = ngDiscovery;
            _jsonOptions = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            };
            _httpClientService = httpClientService;
        }

        public MbpHttpResponse<TResult> Get<TResult>(string serviceName, string relativePath, IDictionary<string, string> parameters, IDictionary<string, string> heads = null) where TResult : class
        {
            return GetAsync<TResult>(serviceName, relativePath, parameters, heads).Result;
        }

        public async Task<MbpHttpResponse<TResult>> GetAsync<TResult>(string serviceName, string relativePath, IDictionary<string, string> parameters, IDictionary<string, string> heads = null) where TResult : class
        {
            var response = await _httpClientService.GetHttpClient().SendAsync(_httpClientService.GetHttpRequest(HttpMethod.Get, _ngDiscovery.GetWebApiServiceUrl(serviceName) + relativePath, parameters, heads));

            return await _httpClientService.GetResponseMessage<TResult>(response);
        }

        public MbpHttpResponse<TResult> Post<TResult, TContent>(string serviceName, string relativePath, TContent content, IDictionary<string, string> heads = null)
            where TResult : class
            where TContent : class
        {
            return PostAsync<TResult, TContent>(serviceName, relativePath, content, heads).Result;
        }

        public async Task<MbpHttpResponse<TResult>> PostAsync<TResult, TContent>(string serviceName, string relativePath, TContent content, IDictionary<string, string> heads = null)
            where TResult : class
            where TContent : class
        {
            var request = _httpClientService.GetHttpRequest(HttpMethod.Post, _ngDiscovery.GetWebApiServiceUrl(serviceName) + relativePath, null, heads);

            request.Content = new StringContent(JsonSerializer.Serialize(content, _jsonOptions), Encoding.UTF8, "application/json");

            var client = _httpClientService.GetHttpClient();

            var response = await client.SendAsync(request);
            return await _httpClientService.GetResponseMessage<TResult>(response);
        }
    }
}
