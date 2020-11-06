using System.Threading.Tasks;
using SonyAudioControl.Model;

namespace SonyAudioControl.Services.SystemControl
{
    public interface ISystemControl
    {
        Task<PowerStatus> GetPowerStatusAsync(string deviceUrl);

        Task SetPowerStatusAsync(string deviceUrl, bool powerOn);
    }
}
