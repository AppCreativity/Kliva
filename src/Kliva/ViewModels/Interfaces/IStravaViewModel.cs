using Kliva.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Windows.UI.Xaml;

namespace Kliva.ViewModels.Interfaces
{
    public interface IStravaViewModel : INotifyPropertyChanged
    {
        ActivityIncrementalCollection ActivityIncrementalCollection { get; set; }
        ActivitySummary SelectedActivity { get; set; }

        /// <summary>
        /// Property needed to know if we need to actually navigate to a different page or not
        /// Depending on the actual visual state we are in
        /// </summary>
        VisualState CurrentState { get; set; }

        /// <summary>
        /// This method will be called by the view when an activity is clicked in a list with
        /// selection mode disabled.
        /// </summary>
        /// <param name="selectedActivity"></param>
        void ActivityInvoked(ActivitySummary selectedActivity);
    }
}
