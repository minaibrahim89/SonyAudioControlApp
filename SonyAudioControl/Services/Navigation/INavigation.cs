using System.Threading.Tasks;
using SonyAudioControl.ViewModels.Base;
using Windows.UI.Xaml.Media.Animation;

namespace SonyAudioControl.Services.Navigation
{
    public interface INavigation
    {
        Task InitializeAsync();

        Task NavigateToAsync<TViewModel>(object data = null, NavigationTransitionInfo transitionInfo = null) where TViewModel : ViewModelBase;

        Task RemoveLastFromBackStackAsync();

        Task NavigateBackAsync();
    }
}
