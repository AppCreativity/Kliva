using Windows.UI.Xaml.Controls;
using Kliva.ViewModels;

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
    }
}
