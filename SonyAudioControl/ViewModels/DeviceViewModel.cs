using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using SonyAudioControl.Model;
using SonyAudioControl.Model.UI;
using SonyAudioControl.ViewModels.Base;

namespace SonyAudioControl.ViewModels
{
    public class DeviceViewModel : ViewModelBase
    {
        private readonly Uri _deviceDescriptionLocation;

        public DeviceViewModel(Device device, Uri deviceDescriptionLocation)
        {
            _deviceDescriptionLocation = deviceDescriptionLocation;
            Name = device.FriendlyName;
            ModelName = device.ModelName;

            var largeIcon = GetDeviceIcon(device, true);
            LargeIconUrl = largeIcon.Url;
            LargeIconWidth = int.Parse(largeIcon.Width);
            LargeIconHeight = int.Parse(largeIcon.Height);

            var smallIcon = GetDeviceIcon(device, false);
            SmallIconUrl = smallIcon.Url;
            SmallIconWidth = int.Parse(smallIcon.Width);
            SmallIconHeight = int.Parse(smallIcon.Height);

            BaseUrl = device.X_ScalarWebAPI_DeviceInfo.X_ScalarWebAPI_BaseURL;

            SelectDeviceCommand = new Command(async () => await SelectDeviceAsync());
        }

        public string Name { get; }

        public string ModelName { get; }

        public string LargeIconUrl { get; }
        public int LargeIconWidth { get; }
        public int LargeIconHeight { get; }

        public string SmallIconUrl { get; }
        public int SmallIconWidth { get; }
        public int SmallIconHeight { get; }

        public string BaseUrl { get; }

        public ICommand SelectDeviceCommand { get; }

        private Icon GetDeviceIcon(Device device, bool large)
        {
            var icons = device.IconList.Icons
                .Where(icon => icon.Mimetype == "image/jpeg")
                .OrderByDescending(icon => icon.Width)
                .Select(icon => new Icon
                {
                    Url = $"http://{_deviceDescriptionLocation.Authority}{icon.Url}",
                    Mimetype = icon.Mimetype,
                    Depth = icon.Depth,
                    Width = icon.Width,
                    Height = icon.Height
                });

            return large ? icons.FirstOrDefault() : icons.LastOrDefault();
        }

        private async Task SelectDeviceAsync()
        {
            await Navigation.NavigateToAsync<DeviceControlViewModel>(this);
        }
    }
}
