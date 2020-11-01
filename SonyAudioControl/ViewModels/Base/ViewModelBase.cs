using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using SonyAudioControl.Services.Navigation;

namespace SonyAudioControl.ViewModels.Base
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        private bool _isBusy;

        public event PropertyChangedEventHandler PropertyChanged;

        public ViewModelBase()
        {
            Navigation = ViewModelLocator.Resolve<INavigation>();
        }

        protected INavigation Navigation { get; }

        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        public virtual Task InitializeAsync(object args)
        {
            return Task.CompletedTask;
        }

        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName] string propertyName = "",
            Action<T> onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke(value);
            OnPropertyChanged(propertyName);
            return true;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
