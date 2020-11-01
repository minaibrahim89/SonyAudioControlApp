using System.Threading.Tasks;
using SonyAudioControl.Model;

namespace SonyAudioControl.Services.Http
{
    public interface IHttpRequestProvider
    {
        Task<T> PostAsync<T>(string url, DeviceRequest request) where T : class;
    }
}
