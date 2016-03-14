using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Kliva.Services;
using Kliva.Services.Interfaces;
using Microsoft.Practices.ServiceLocation;
using Windows.ApplicationModel;

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

            if (DesignMode.DesignModeEnabled)
            {
                SimpleIoc.Default.Register<INavigationService, DesignModeNavigationService>();
            }
            else
            {
                SimpleIoc.Default.Register<INavigationService, NavigationService>();
            }
            
            SimpleIoc.Default.Register<IMessenger, Messenger>();
            SimpleIoc.Default.Register<IMessageBoxService, MessageBoxService>();
            SimpleIoc.Default.Register<IStorageService, StorageService>();
            SimpleIoc.Default.Register<ISettingsService, SettingsService>();
            SimpleIoc.Default.Register<IStravaService, StravaService>();
            SimpleIoc.Default.Register<IStravaActivityService, StravaActivityService>();
            SimpleIoc.Default.Register<IStravaAthleteService, StravaAthleteService>();
            SimpleIoc.Default.Register<IStravaClubService, StravaClubService>();
            SimpleIoc.Default.Register<IStravaSegmentService, StravaSegmentService>();

            Register<MainViewModel>();
            Register<ActivityDetailViewModel>();
            Register<LoginViewModel>();
            Register<SettingsViewModel>();
            Register<SidePaneViewModel>();
            Register<ClubsViewModel>();
            Register<ClubDetailViewModel>();
            Register<ProfileViewModel>();
            Register<StatsViewModel>();

            Register<StravaWebClient>(); // singleton (default in SimpleIoC)
        }

        public MainViewModel Main => Get<MainViewModel>();
        public ActivityDetailViewModel ActivityDetail => Get<ActivityDetailViewModel>();
        public LoginViewModel Login => Get<LoginViewModel>();
        public SettingsViewModel Settings => Get<SettingsViewModel>();
        public SidePaneViewModel SidePane => Get<SidePaneViewModel>();
        public ClubsViewModel Clubs => Get<ClubsViewModel>();
        public ClubDetailViewModel ClubDetail => Get<ClubDetailViewModel>();
        public ProfileViewModel Profile => Get<ProfileViewModel>();
        public StatsViewModel Stats => Get<StatsViewModel>();
    }
}
