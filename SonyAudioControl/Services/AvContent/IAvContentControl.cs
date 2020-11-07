using System.Threading.Tasks;
using SonyAudioControl.Model;

namespace SonyAudioControl.Services.AvContent
{
    public interface IAvContentControl
    {
        Task<PlaybackSource[]> GetSourceListAsync(string deviceUrl, string scheme);

        Task<PlaybackContent> GetPlayingContentInfoAsync(string deviceUrl);

        Task SetPlayContentAsync(string deviceUrl, string source);

        Task TogglePausePlayingContentAsync(string deviceUrl);

        Task SetPlayNextContentAsync(string deviceUrl);
        
        Task SetPlayPreviousContentAsync(string deviceUrl);

        Task ScanPlayingContentAsync(string deviceUrl, bool backward);
    }
}
