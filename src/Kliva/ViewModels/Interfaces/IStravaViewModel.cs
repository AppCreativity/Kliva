using Kliva.Models;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;

namespace Kliva.ViewModels.Interfaces
{
    public interface IStravaViewModel
    {
        ObservableCollection<ActivitySummary> Activities { get; set; }
        ActivitySummary SelectedActivity { get; set; }

        /// <summary>
        /// Property needed to know if we need to actually navigate to a different page or not
        /// Depending on the actual visual state we are in
        /// </summary>
        VisualState CurrentState { get; set; }
    }
}
