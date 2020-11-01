﻿using System.Threading.Tasks;
using SonyAudioControl.Model;

namespace SonyAudioControl.Services.AudioControl
{
    public interface IAudioControl
    {
        Task<VolumeInfo> GetVolumeInfoAsync(string deviceUrl);

        Task SetVolumeAsync(string deviceUrl, int volume);

        Task SetMuteAsync(string deviceUrl, bool mute);
    }
}
