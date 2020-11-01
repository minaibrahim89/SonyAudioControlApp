using System;
using Autofac;
using SonyAudioControl.Services.AudioControl;
using SonyAudioControl.Services.Discovery;
using SonyAudioControl.Services.Http;
using SonyAudioControl.Services.Navigation;
using SonyAudioControl.Services.Notification;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SonyAudioControl.ViewModels.Base
{
    public static class ViewModelLocator
    {
        public enum AutoWireViewModelMode
        {
            Disable,
            Enable,
            EnableAndInitialize
        }

        public static readonly DependencyProperty AutoWireViewModelProperty = DependencyProperty.RegisterAttached(
            "AutoWireViewModel",
            typeof(AutoWireViewModelMode),
            typeof(ViewModelLocator),
            new PropertyMetadata(default(AutoWireViewModelMode), new PropertyChangedCallback(OnAutoWireViewModelChanged)));

        private static IContainer _container;

        public static AutoWireViewModelMode GetAutoWireViewModel(DependencyObject bindable)
        {
            return (AutoWireViewModelMode)bindable.GetValue(AutoWireViewModelProperty);
        }

        public static void SetAutoWireViewModel(DependencyObject bindable, AutoWireViewModelMode value)
        {
            bindable.SetValue(AutoWireViewModelProperty, value);
        }

        static ViewModelLocator()
        {
            _container = new ContainerBuilder()
                .RegisterServices()
                .RegisterViewModels()
                .Build();
        }

        private static ContainerBuilder RegisterServices(this ContainerBuilder builder)
        {
            builder.RegisterType<Navigation>().As<INavigation>().SingleInstance();
            builder.RegisterType<DeviceFinder>().As<IDeviceFinder>().SingleInstance();
            builder.RegisterType<AudioControl>().As<IAudioControl>().SingleInstance();
            builder.RegisterType<HttpRequestProvider>().As<IHttpRequestProvider>().SingleInstance();
            builder.RegisterType<NotificationListener>().As<INotificationListener>().SingleInstance();

            return builder;
        }

        private static ContainerBuilder RegisterViewModels(this ContainerBuilder builder)
        {
            builder.RegisterType<DiscoveryViewModel>().AsSelf();
            builder.RegisterType<DeviceControlViewModel>().AsSelf();

            return builder;
        }

        public static T Resolve<T>()
            where T : class
        {
            return _container.Resolve<T>();
        }

        private static void OnAutoWireViewModelChanged(DependencyObject bindable, DependencyPropertyChangedEventArgs e)
        {
            if (!(bindable is Page view))
                return;

            var autoWireMode = (AutoWireViewModelMode)e.NewValue;
            if (autoWireMode == AutoWireViewModelMode.Disable)
                return;

            var viewType = view.GetType();
            var viewAssemblyName = viewType.Assembly.FullName;
            var viewModelName = $"{viewType.FullName?.Replace(".Views.", ".ViewModels.")}Model, {viewAssemblyName}";

            var viewModelType = Type.GetType(viewModelName);
            if (viewModelType == null)
                return;

            var viewModel = (ViewModelBase)_container.Resolve(viewModelType);
            view.DataContext = viewModel;

            if (autoWireMode == AutoWireViewModelMode.EnableAndInitialize)
                viewModel.InitializeAsync(null);
        }
    }
}
