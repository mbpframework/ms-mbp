using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace Mbp.Net.Http
{
    internal class MbpHttpClient : IMbpHttpClient
    {
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly IHttpClientService _httpClientService;

        public MbpHttpClient(IHttpClientService httpClientService)
        {
            _jsonOptions = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true,
                AllowTrailingCommas = true
            };
            _httpClientService = httpClientService;
        }

        public MbpHttpResponse<TResult> Get<TResult>(string serviceBaseUrl, string relativePath, IDictionary<string, string> parameters, IDictionary<string, string> heads = null) where TResult : class
        {
            return GetAsync<TResult>(serviceBaseUrl, relativePath, parameters, heads).Result;
        }

        public async Task<MbpHttpResponse<TResult>> GetAsync<TResult>(string serviceBaseUrl, string relativePath, IDictionary<string, string> parameters, IDictionary<string, string> heads = null) where TResult : class
        {
            return await GetRequest<TResult>(EndWithSlash(serviceBaseUrl) + StartWithSlash(relativePath), parameters, heads);
        }

        public async Task<MbpHttpResponse<TResult>> GetAsync<TResult>(string url, IDictionary<string, string> parameters, IDictionary<string, string> heads = null) where TResult : class
        {
            return await GetRequest<TResult>(url, parameters, heads);
        }

        private async Task<MbpHttpResponse<TResult>> GetRequest<TResult>(string url, IDictionary<string, string> parameters, IDictionary<string, string> heads) where TResult : class
        {
            var response = await _httpClientService.GetHttpClient().SendAsync(_httpClientService.GetHttpRequest(HttpMethod.Get, url, parameters, heads));
            return await _httpClientService.GetResponseMessage<TResult>(response);
        }

        public MbpHttpResponse<TResult> Post<TResult, TContent>(string serviceBaseUrl, string relativePath, TContent content, IDictionary<string, string> heads = null)
            where TResult : class
            where TContent : class
        {
            return PostAsync<TResult, TContent>(serviceBaseUrl, relativePath, content, heads).Result;
        }

        public async Task<MbpHttpResponse<TResult>> PostAsync<TResult, TContent>(string serviceBaseUrl, string relativePath, TContent content, IDictionary<string, string> heads = null)
            where TResult : class
            where TContent : class
        {
            return await PostRequest<TResult, TContent>(EndWithSlash(serviceBaseUrl) + StartWithSlash(relativePath), content, heads);
        }

        public async Task<MbpHttpResponse<TResult>> PostAsync<TResult, TContent>(string url, TContent content, IDictionary<string, string> heads = null)
            where TResult : class
            where TContent : class
        {
            return await PostRequest<TResult, TContent>(url, content, heads);
        }

        private async Task<MbpHttpResponse<TResult>> PostRequest<TResult, TContent>(string url, TContent content, IDictionary<string, string> heads)
            where TResult : class
            where TContent : class
        {
            var request = _httpClientService.GetHttpRequest(HttpMethod.Post, url, null, heads);

            request.Content = new StringContent(JsonSerializer.Serialize(content, _jsonOptions), Encoding.UTF8, "application/json");

            var client = _httpClientService.GetHttpClient();

            var response = await client.SendAsync(request);

            return await _httpClientService.GetResponseMessage<TResult>(response);
        }

        private string EndWithSlash(string source)
        {
            if (!source.EndsWith("/"))
                source += "/";
            return source;
        }

        private string StartWithSlash(string source)
        {
            if (source.StartsWith("/"))
                source = source.Remove(0, 1);
            return source;
        }
    }
}
