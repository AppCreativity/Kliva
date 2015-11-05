using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Cimbalino.Toolkit.Services;
using Kliva.Messages;
using Kliva.Models;
using Kliva.ViewModels.Interfaces;

namespace Kliva.ViewModels
{
    public class ActivityDetailViewModel : KlivaBaseViewModel, IStravaViewModel
    {
        public ActivityDetailViewModel(INavigationService navigationService) : base(navigationService)
        {
            MessengerInstance.Register<ActivitySummaryMessage>(this, item =>
            {
                SelectedActivity = item.ActivitySummary;
            });
        }

        public ObservableCollection<ActivitySummary> Activities { get; set; }

        private ActivitySummary _selectedActivity;
        public ActivitySummary SelectedActivity
        {
            get { return _selectedActivity; }
            set { Set(() => SelectedActivity, ref _selectedActivity, value); }
        }
        public VisualState CurrentState { get; set; }
    }
}
