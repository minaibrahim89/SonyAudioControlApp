using SonyAudioControl.ViewModels;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SonyAudioControl.Controls
{
    public sealed partial class UpperToolbar : UserControl
    {
        public UpperToolbar()
        {
            InitializeComponent();
        }

        private DeviceControlViewModel ViewModel => (DeviceControlViewModel)DataContext;

        internal void LoadSourceSelectorOptions()
        {
            if (ViewModel?.InputControl.SourceList == null)
                return;

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
