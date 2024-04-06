using System.Text;
using System.Text.Json;

namespace RealityPlus.Test.Clients
{
    internal abstract class BaseClient
    {
        protected abstract string BaseUrl { get; } 
 
        private JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        private string BuildUrl(string extraUrl)
        {
            if (string.IsNullOrWhiteSpace(extraUrl))
            {
                return BaseUrl;
            }
            return $"{BaseUrl}/{extraUrl}".Trim('/');
        }

        protected async Task<TResponse?> Post<TRequest, TResponse>(string url, TRequest postData, Dictionary<string, string>? headers)
        {
            HttpClient client = BuildHttpClient(headers);

            var json = JsonSerializer.Serialize(postData, JsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var reponse = await client.PostAsync(BuildUrl(url), content);
            return await DeserializeResponse<TResponse>(reponse);
        }

        protected async Task<TResponse?> Get<TResponse>(string url, Dictionary<string, string>? headers)
        {
            HttpClient client = BuildHttpClient(headers);
            var response = await client.GetAsync(BuildUrl(url));
            return await DeserializeResponse<TResponse>(response);
        }

        protected async Task Delete(string url, Dictionary<string, string>? headers)
        {
            HttpClient client = BuildHttpClient(headers);
            var response = await client.DeleteAsync(BuildUrl(url));
            await DeserializeResponse<string>(response);
        }

        private HttpClient BuildHttpClient(Dictionary<string, string>? headers)
        {
            HttpClient client = new HttpClient();
            if (headers != null)
            {
                foreach (var (key, value) in headers)
                {
                    client.DefaultRequestHeaders.Add(key, value);
                }
            }
            return client;
        }

        private async Task<TResponse?> DeserializeResponse<TResponse>(HttpResponseMessage response)
        {
            if (response == null)
            {
                return default;
            }
            var body = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(body))
            {
                return default;
            }
            try
            {
                return JsonSerializer.Deserialize<TResponse>(body, JsonOptions);
            }
            catch
            {
                throw new Exception(body);
            }
        }
    }
}
