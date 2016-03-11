using Cimbalino.Toolkit.Services;
using Kliva.Services.Interfaces;

namespace Kliva.ViewModels
{
    public class SettingsViewModel : KlivaBaseViewModel
    {
        private readonly ISettingsService _settingsService;

        public string AppVersion => _settingsService.AppVersion.ToString();

        public SettingsViewModel(INavigationService navigationService, ISettingsService settingsService) : base(navigationService)
        {
            _settingsService = settingsService;
        }
    }
}
