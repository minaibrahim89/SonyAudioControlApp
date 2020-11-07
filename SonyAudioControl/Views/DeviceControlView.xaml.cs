using SonyAudioControl.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

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

        public DeviceControlViewModel ViewModel => (DeviceControlViewModel)DataContext;

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            var viewModel = (DeviceControlViewModel)DataContext;
            await viewModel.InitializeAsync(e.Parameter);

            LoadSourceSelectorOptions();
        }

        private void LoadSourceSelectorOptions()
        {
            var menuFlyout = new MenuFlyout();

            foreach (var source in ViewModel.InputControl.SourceList)
            {
                menuFlyout.Items.Add(new MenuFlyoutItem
                {
                    Text = source.Title,
                    Command = ViewModel.InputControl.SelectSourceCommand,
                    CommandParameter = source.Source
                });
            }
            
            InputSelector.Flyout = menuFlyout;
        }
    }
}
