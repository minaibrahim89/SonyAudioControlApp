using System;
using System.Globalization;
using System.Threading.Tasks;
using SonyAudioControl.ViewModels;
using SonyAudioControl.ViewModels.Base;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace SonyAudioControl.Services.Navigation
{
    public class Navigation : INavigation
    {
        private Frame MainFrame => Window.Current.Content as Frame;

        public async Task InitializeAsync()
        {
            await NavigateToAsync<DiscoveryViewModel>(new EntranceNavigationTransitionInfo());
        }

        public Task NavigateToAsync<TViewModel>(object data = null, NavigationTransitionInfo transitionInfo = null)
            where TViewModel : ViewModelBase
        {
            return NavigateToAsync(typeof(TViewModel), data, transitionInfo ?? new CommonNavigationTransitionInfo());
        }

        private async Task NavigateToAsync(Type viewModelType, object data, NavigationTransitionInfo transitionInfo)
        {
            if (!typeof(ViewModelBase).IsAssignableFrom(viewModelType))
                return;

            var pageType = GetPageTypeForViewModel(viewModelType);

            MainFrame.Navigate(pageType, data, transitionInfo);
        }

        private Type GetPageTypeForViewModel(Type viewModelType)
        {
            var viewName = viewModelType.FullName.Replace("Model", string.Empty);
            var viewModelAssemblyName = viewModelType.Assembly.FullName;
            var viewAssemblyName = string.Format(CultureInfo.InvariantCulture, "{0}, {1}", viewName, viewModelAssemblyName);
            var viewType = Type.GetType(viewAssemblyName);

            return viewType;
        }

        public Task RemoveLastFromBackStackAsync()
        {
            if (MainFrame != null)
                MainFrame.BackStack.RemoveAt(MainFrame.BackStackDepth - 1);

            return Task.CompletedTask;
        }

        public Task NavigateBackAsync()
        {
            MainFrame.GoBack();

            return Task.CompletedTask;
        }
    }
}
