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

        protected async Task<TResponse?> PostMessage<TRequest, TResponse>(string url, TRequest postData)
        {
            HttpClient client = new HttpClient();

            var json = JsonSerializer.Serialize(postData, JsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var reponse = await client.PostAsync(BuildUrl(url), content);
            return await DeserializeResponse<TResponse>(reponse);
        }

        protected async Task<TResponse?> GetMessage<TResponse>(string url)
        {
            HttpClient client = new HttpClient();

            var response = await client.GetAsync(BuildUrl(url));
            return await DeserializeResponse<TResponse>(response);
        }

        private async Task<TResponse?> DeserializeResponse<TResponse>(HttpResponseMessage response)
        {
            if (response == null)
            {
                return default;
            }
            var body = await response.Content.ReadAsStringAsync();

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
