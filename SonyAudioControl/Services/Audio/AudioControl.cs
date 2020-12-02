using System.IO;
using System.Threading.Tasks;
using SonyAudioControl.Model;
using SonyAudioControl.Services.Http;

namespace SonyAudioControl.Services.Audio
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

            var response = await _httpRequestProvider.PostAsync<VolumeInfo[]>($"{deviceUrl}/audio", request);

            return response[0];
        }

        public async Task SetVolumeAsync(string deviceUrl, int volume)
        {
            var request = new DeviceRequest
            {
                Method = "setAudioVolume",
                Version = "1.1",
                Params = new[] { new { output = "", volume = volume.ToString() } }
            };

            await _httpRequestProvider.PostAsync<object>($"{deviceUrl}/audio", request);
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

        public async Task<SoundSetting[]> GetSoundSettingsAsync(string deviceUrl)
        {
            var request = new DeviceRequest
            {
                Method = "getSoundSettings",
                Version = "1.1",
                Params = new[] { new { target = "" } }
            };

            return await _httpRequestProvider.PostAsync<SoundSetting[]>(Path.Combine(deviceUrl, "audio"), request);
        }

        public async Task SetSoundSettingAsync(string deviceUrl, string target, string value)
        {
            var request = new DeviceRequest
            {
                Method = "setSoundSettings",
                Version = "1.1",
                Params = new[]
                {
                    new
                    {
                        settings = new[] { new { target, value } }
                    }
                }
            };

            await _httpRequestProvider.PostAsync<object>(Path.Combine(deviceUrl, "audio"), request);
        }
    }
}
