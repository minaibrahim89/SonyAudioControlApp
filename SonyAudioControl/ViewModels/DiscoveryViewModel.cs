using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using SonyAudioControl.Extensions;
using SonyAudioControl.Model;
using SonyAudioControl.Model.UI;
using SonyAudioControl.Services.Discovery;
using SonyAudioControl.ViewModels.Base;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace SonyAudioControl.ViewModels
{
    public class DiscoveryViewModel : ViewModelBase
    {
        private readonly IDeviceFinder _deviceFinder;
        private bool _devicesFound;

        public DiscoveryViewModel(IDeviceFinder deviceFinder)
        {
            _deviceFinder = deviceFinder;

            RetryCommand = new Command(async () => await TrySearchForDevicesAsync());
        }

        public bool DevicesFound
        {
            get => _devicesFound;
            set => SetProperty(ref _devicesFound, value);
        }

        public ObservableCollection<DeviceViewModel> Devices { get; }
            = new ObservableCollection<DeviceViewModel>();

        public ICommand RetryCommand { get; }

        public override async Task InitializeAsync(object args)
        {
            await TrySearchForDevicesAsync();
        }

        private async Task TrySearchForDevicesAsync()
        {
            try
            {
                IsBusy = true;
                await Task.Run(SearchForDevicesAsync);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task SearchForDevicesAsync()
        {
            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            IEnumerable<DeviceDescription> deviceDescriptions;

            if (!localSettings.Values.TryGetValue("devices", out var cachedDevices) || (string)cachedDevices == "[]")
            {
                deviceDescriptions = await _deviceFinder.SearchForUpnpDevicesAsync();
                localSettings.Values["devices"] = deviceDescriptions.ToJson();
            }
            else
            {
                deviceDescriptions = ((string)cachedDevices).DeJson<IEnumerable<DeviceDescription>>();
            }

            var viewModels = deviceDescriptions.Select(d => new DeviceViewModel(d.Device, d.DescriptionLocation)).ToList();

            _ = CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    Devices.AddRange(viewModels);
                    DevicesFound = Devices.Count > 0;

                    if (Devices.Count == 1)
                        Devices[0].SelectDeviceCommand.Execute(null);
                });
        }
    }
}
