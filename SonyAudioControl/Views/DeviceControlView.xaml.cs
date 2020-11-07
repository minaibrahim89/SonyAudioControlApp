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

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            var viewModel = (DeviceControlViewModel)DataContext;
            await viewModel.InitializeAsync(e.Parameter);

            UpperToolbar.LoadSourceSelectorOptions();
        }        
    }
}
