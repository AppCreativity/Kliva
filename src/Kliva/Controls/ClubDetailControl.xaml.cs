using Windows.UI.Xaml.Controls;
using Kliva.ViewModels;

namespace Kliva.Controls
{
    public sealed partial class ClubDetailControl : UserControl
    {
        public ClubDetailViewModel ViewModel => DataContext as ClubDetailViewModel;

        public ClubDetailControl()
        {
            InitializeComponent();
        }
    }
}
