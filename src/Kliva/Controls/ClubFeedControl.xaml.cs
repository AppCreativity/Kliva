using Windows.UI.Xaml.Controls;
using Kliva.ViewModels;
using Windows.UI.Xaml;
using Kliva.Models;
using System.Linq;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Kliva.Controls
{
    public sealed partial class ClubFeedControl : UserControl
    {
        private ClubsViewModel ViewModel => DataContext as ClubsViewModel;

        public ClubFeedControl()
        {
            this.InitializeComponent();
            DataContextChanged += (sender, args) => Bindings.Update();
        }

        private void AdaptiveStates_CurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            if (IsDesktopState())
            {
                // Make sure the selected item is properly restored if we switch to the wide state
                ClubList.SelectionMode = ListViewSelectionMode.Single;
                ClubList.SelectedItem = ViewModel.SelectedClub;
            }
        }

        private bool IsDesktopState()
        {
            return AdaptiveStates.CurrentState.Name == Desktop.Name;
        }

        private void ClubList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsDesktopState())
            {
                // Only update the ViewModel if we are in the wide state where the list supports
                // selection
                ViewModel.SelectedClub = (ClubSummary)e.AddedItems.FirstOrDefault();
            }
        }

        private void ClubList_ItemClick(object sender, ItemClickEventArgs e)
        {
            ViewModel.ClubInvoked((ClubSummary)e.ClickedItem);
        }
    }
}
