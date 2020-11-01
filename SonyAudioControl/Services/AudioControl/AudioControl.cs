using System.IO;
using System.Threading.Tasks;
using SonyAudioControl.Model;
using SonyAudioControl.Services.Http;

namespace SonyAudioControl.Services.AudioControl
{
    public class AudioControl : IAudioControl
    {
        private readonly IHttpRequestProvider _httpRequestProvider;

        public AudioControl(IHttpRequestProvider httpRequestProvider)
        {
            _httpRequestProvider = httpRequestProvider;
        }

        public async Task<VolumeInfo> GetVolumeInfoAsync(string deviceUrl)
        {
            var request = new DeviceRequest
            {
                Method = "getVolumeInformation",
                Version = "1.1",
                Params = new[] { new { output = "" } }
            };

            var response = await _httpRequestProvider.PostAsync<VolumeInfo>(Path.Combine(deviceUrl, "audio"), request);

            return response;
        }

        public async Task SetVolumeAsync(string deviceUrl, int volume)
        {
            var request = new DeviceRequest
            {
                Method = "setAudioVolume",
                Version = "1.1",
                Params = new[] { new { output = "", volume = volume.ToString() } }
            };

            await _httpRequestProvider.PostAsync<object>(Path.Combine(deviceUrl, "audio"), request);
        }

        public async Task SetMuteAsync(string deviceUrl, bool mute)
        {
            var request = new DeviceRequest
            {
                Method = "setAudioMute",
                Version = "1.1",
                Params = new[] { new { mute = mute ? "on" : "off" } }
            };

            await _httpRequestProvider.PostAsync<object>(Path.Combine(deviceUrl, "audio"), request);
        }
    }
}
