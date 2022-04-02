using Flurl;
using Microsoft.Win32.SafeHandles;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Runtime.InteropServices;
using System.Text;
using Megatokyo.Infrastructure.Repository.EF;

namespace Megatokyo.Server.UnitTest
{
    internal class APIClient : IDisposable
    {
        bool disposed = false;
        readonly SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);
        private readonly WebApplicationTemplateFactory _application;
        private readonly HttpClient _client;
        private readonly string _apiRoot;

        public Action<APIContext>? InsertData { get { return _application.InsertData; } set { _application.InsertData = value; } }

        internal APIClient(string apiRoot)
        {
            _application = new WebApplicationTemplateFactory();
            _client = _application.CreateClient();
            _apiRoot = apiRoot;
        }

        internal async Task<T?> PostAsync<T>(string apiUrl, object value)
        {
            HttpResponseMessage? response = await _client.PostAsJsonAsync(_apiRoot.AppendPathSegment(apiUrl), value);
            if (response != null && response.IsSuccessStatusCode)
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();
                if (jsonResponse != null)
                {
                    return JsonConvert.DeserializeObject<T>(jsonResponse);
                }
            }
            return default;
        }

        internal async Task<T?> GetAsync<T>(string apiUrl)
        {
            return await _client.GetFromJsonAsync<T>(_apiRoot.AppendPathSegment(apiUrl));
        }

        internal async Task<T?> PutAsync<T>(string apiUrl, object value)
        {
            HttpResponseMessage? response = await _client.PutAsJsonAsync(_apiRoot.AppendPathSegment(apiUrl), value);
            if (response != null && response.IsSuccessStatusCode)
            {
                if (typeof(T) == typeof(bool))
                    return (T)(object)true;
                string jsonResponse = await response.Content.ReadAsStringAsync();
                if (jsonResponse != null)
                {
                    return JsonConvert.DeserializeObject<T>(jsonResponse);
                }
            }
            if (typeof(T) == typeof(bool))
                return (T)(object)false;
            return default;
        }

        internal async Task<T?> PatchAsync<T>(string apiUrl, object value)
        {
            StringContent content = new(JsonConvert.SerializeObject(value), Encoding.UTF8, "application/json-patch+json");
            HttpResponseMessage? response = await _client.PatchAsync(_apiRoot.AppendPathSegment(apiUrl), content);
            if (response != null && response.IsSuccessStatusCode)
            {
                if (typeof(T) == typeof(bool))
                    return (T)(object)true;
                string jsonResponse = await response.Content.ReadAsStringAsync();
                if (jsonResponse != null)
                {
                    return JsonConvert.DeserializeObject<T>(jsonResponse);
                }
            }
            if (typeof(T) == typeof(bool))
                return (T)(object)false;
            return default;
        }

        internal async Task<bool> DeleteAsync(string apiUrl)
        {
            HttpResponseMessage? response = await _client.DeleteAsync(_apiRoot.AppendPathSegment(apiUrl));
            if (response != null && response.IsSuccessStatusCode)
                return true;
            return false;
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                handle.Dispose();
                _application.Dispose();
            }

            disposed = true;
        }
    }
}
