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
using SonyAudioControl.Services.Storage;
using SonyAudioControl.ViewModels.Base;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace SonyAudioControl.ViewModels
{
    public class DiscoveryViewModel : ViewModelBase
    {
        private readonly IDeviceFinder _deviceFinder;
        private readonly IStorage _storage;
        private bool _devicesFound;

        public DiscoveryViewModel(IDeviceFinder deviceFinder, IStorage storage)
        {
            _deviceFinder = deviceFinder;
            _storage = storage;

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

                await Task.Run(LoadOrSearchForDevicesAsync);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task LoadOrSearchForDevicesAsync()
        {
            var savedDevices = await GetSavedDevicesAsync();

            if (savedDevices.IsNullOrEmpty())
                savedDevices = await SearchForDevicesAsync();

            await SetDevicesAsync(savedDevices);
        }

        private async Task<IEnumerable<DeviceViewModel>> GetSavedDevicesAsync()
        {
            if (!await _storage.TryGetAsync<DeviceDescription[]>("devices", out var devices) || devices.Length == 0)
                return Array.Empty<DeviceViewModel>();

            return devices.Select(d => new DeviceViewModel(d.Device, d.DescriptionLocation));
        }

        private async Task<IEnumerable<DeviceViewModel>> SearchForDevicesAsync()
        {
            var deviceDescriptions = await _deviceFinder.SearchForUpnpDevicesAsync();

            return deviceDescriptions.Select(d => new DeviceViewModel(d.Device, d.DescriptionLocation));
        }

        private async Task SetDevicesAsync(IEnumerable<DeviceViewModel> viewModels)
        {
            await CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
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
