﻿using System.Threading.Tasks;
using SonyAudioControl.Model;
using SonyAudioControl.Services.Http;

namespace SonyAudioControl.Services.System
{
    public class SystemControl : ISystemControl
    {
        private readonly IHttpRequestProvider _httpRequestProvider;

        public SystemControl(IHttpRequestProvider httpRequestProvider)
        {
            _httpRequestProvider = httpRequestProvider;
        }

        public async Task<SystemInformation> GetSystemInformationAsync(string deviceUrl)
        {
            var request = new DeviceRequest
            {
                Method = "getSystemInformation",
                Version = "1.4"
            };

            return await _httpRequestProvider.PostAsync<SystemInformation>($"{deviceUrl}/system", request);
        }

        public async Task<PowerStatus> GetPowerStatusAsync(string deviceUrl)
        {
            var request = new DeviceRequest
            {
                Method = "getPowerStatus",
                Version = "1.1"
            };

            return await _httpRequestProvider.PostAsync<PowerStatus>($"{deviceUrl}/system", request);
        }

        public async Task SetPowerStatusAsync(string deviceUrl, bool powerOn)
        {
            var request = new DeviceRequest
            {
                Method = "setPowerStatus",
                Version = "1.1",
                Params = new[] { new { status = powerOn ? "active" : "standby" } }
            };

            await _httpRequestProvider.PostAsync<object>($"{deviceUrl}/system", request);
        }
    }
}
