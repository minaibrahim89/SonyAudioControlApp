using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using SonyAudioControl.Extensions;
using SonyAudioControl.Model;

namespace SonyAudioControl.Services.Http
{
    public class HttpRequestProvider : IHttpRequestProvider
    {
        private readonly HttpClient _httpClient;

        public HttpRequestProvider()
        {
            var handler = new HttpClientHandler
            {
                UseDefaultCredentials = true
            };

            _httpClient = new HttpClient(handler);
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<T> PostAsync<T>(string url, DeviceRequest request)
            where T : class
        {
            try
            {
                var response = await _httpClient.PostAsync(url, new StringContent(request.ToJson(), Encoding.UTF8, "application/json"));

                if (!response.IsSuccessStatusCode)
                    return default;

                var json = await response.Content.ReadAsStringAsync();

                return json.DeJson<DeviceResponse<T>>().Result.FirstOrDefault();
            }
            catch (global::System.Exception ex)
            {
                Debug.WriteLine("Bad request: " + ex);
                throw;
            }
        }
    }
}
