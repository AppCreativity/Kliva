using Windows.UI.Xaml.Controls;
using Kliva.ViewModels;

namespace Kliva.Views
{
    public sealed partial class SegmentPage : Page
    {
        private SegmentViewModel ViewModel => DataContext as SegmentViewModel;

        public SegmentPage()
        {
            this.InitializeComponent();
        }
    }
}
