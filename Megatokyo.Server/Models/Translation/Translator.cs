using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Megatokyo.Server.Models.Translations
{
    internal class Translator : ITranslator
    {
        private const string host = "https://api.cognitive.microsofttranslator.com";
        private const string path = "/translate?api-version=3.0";

        public string ClientKey { get; set; }

        public async Task<string> Translate(string language, string text)
        {
            string uri = host + path + "&to=" + language;
            object[] body = new object[]
            {
                new { Text = text }
            };
            string requestBody = JsonConvert.SerializeObject(body);
            using HttpClient client = new();
            using HttpRequestMessage request = new()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(uri),
                Content = new StringContent(requestBody, Encoding.UTF8, "application/json")
            };
            request.Headers.Add("Ocp-Apim-Subscription-Key", ClientKey);
            HttpResponseMessage response = await client.SendAsync(request);
            string responseBody = await response.Content.ReadAsStringAsync();
            return responseBody;
        }

        public async Task<string> GetLanguages()
        {
            using HttpClient client = new();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", ClientKey);
            UriBuilder uriBuilder = new()
            {
                Host = host,
                Path = path
            };
            HttpResponseMessage response = await client.GetAsync(uriBuilder.Uri);
            string result = await response.Content.ReadAsStringAsync();
            return JsonConvert.SerializeObject(JsonConvert.DeserializeObject(result), Formatting.Indented);
        }
    }
}
