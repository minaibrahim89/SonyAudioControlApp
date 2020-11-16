using System;
using Windows.UI.Xaml;
using SonyAudioControl.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using NavigationView = Microsoft.UI.Xaml.Controls.NavigationView;
using NavigationViewItemInvokedEventArgs = Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SonyAudioControl.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DeviceControlView : Page
    {
        public DeviceControlView()
        {
            InitializeComponent();
        }

        private DeviceControlViewModel ViewModel => DataContext as DeviceControlViewModel;

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await ViewModel.InitializeAsync(e.Parameter);

            UpperToolbar.LoadSourceSelectorOptions();
        }

        private void DeviceControlView_OnLoaded(object sender, RoutedEventArgs e)
        {
            NavigationView.IsPaneOpen = false;
            NavigationView.SelectedItem = SoundSettings;
            ContentFrame.NavigateToType(typeof(SoundSettingsView), null, new FrameNavigationOptions());
        }

        private void NavigationView_OnItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            var navOptions = new FrameNavigationOptions
            {
                TransitionInfoOverride = args.RecommendedNavigationTransitionInfo
            };
            Type pageType = null;
            
            if (args.InvokedItemContainer == SoundSettings)
                pageType = typeof(SoundSettingsView);

            NavigationView.Header = args.InvokedItem;
            ContentFrame.NavigateToType(pageType, null, navOptions);
        }
    }
}
