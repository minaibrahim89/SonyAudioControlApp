﻿using System.Threading.Tasks;
using SonyAudioControl.Model;
using SonyAudioControl.Services.Http;

namespace SonyAudioControl.Services.AvContent
{
    public class AvContentControl : IAvContentControl
    {
        private readonly IHttpRequestProvider _httpRequestProvider;

        public AvContentControl(IHttpRequestProvider httpRequestProvider)
        {
            _httpRequestProvider = httpRequestProvider;
        }

        public async Task<PlaybackSource[]> GetSourceListAsync(string deviceUrl, string scheme)
        {
            var request = new DeviceRequest
            {
                Method = "getSourceList",
                Version = "1.2",
                Params = new[] { new { scheme } }
            };

            return await _httpRequestProvider.PostAsync<PlaybackSource[]>($"{deviceUrl}/avContent", request);
        }

        public async Task<PlaybackContent> GetPlayingContentInfoAsync(string deviceUrl)
        {
            var request = new DeviceRequest
            {
                Method = "getPlayingContentInfo",
                Version = "1.2",
                Params = new[] { new { output = "" } }
            };

            var playbackContents = await _httpRequestProvider.PostAsync<PlaybackContent[]>($"{deviceUrl}/avContent", request);

            return playbackContents[0];
        }

        public async Task SetPlayContentAsync(string deviceUrl, string source)
        {
            var request = new DeviceRequest
            {
                Method = "setPlayContent",
                Version = "1.2",
                Params = new[] { new { uri = source } }
            };

            await _httpRequestProvider.PostAsync<object>($"{deviceUrl}/avContent", request);
        }
    }
}
