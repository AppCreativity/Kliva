using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Ioc;
using Kliva.Services;
using Kliva.Services.Interfaces;
using Microsoft.Practices.ServiceLocation;

namespace Kliva.ViewModels
{
    public class ViewModelLocator
    {
        private static void Register<T>(bool createImmediately = false) where T : class
        {
            SimpleIoc.Default.Register<T>(createImmediately);
        }

        internal static T Get<T>() where T : class
        {
            return SimpleIoc.Default.GetInstance<T>();
        }

        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<INavigationService, NavigationService>();
            SimpleIoc.Default.Register<IMessageBoxService, MessageBoxService>();
            SimpleIoc.Default.Register<IStorageService, StorageService>();
            SimpleIoc.Default.Register<ISettingsService, SettingsService>();
            SimpleIoc.Default.Register<IStravaService, StravaService>();
            SimpleIoc.Default.Register<IStravaActivityService, StravaActivityService>();
            SimpleIoc.Default.Register<IStravaAthleteService, StravaAthleteService>();

            Register<MainViewModel>();
            Register<LoginViewModel>();
            Register<SettingsViewModel>();
        }

        public MainViewModel Main => Get<MainViewModel>();

        public LoginViewModel Login => Get<LoginViewModel>();

        public SettingsViewModel Settings => Get<SettingsViewModel>();
    }
}
