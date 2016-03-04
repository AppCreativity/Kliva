using Windows.UI.Xaml.Controls;
using Kliva.ViewModels;

namespace Kliva.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StatsPage : Page
    {
        private StatsViewModel ViewModel => DataContext as StatsViewModel;

        public StatsPage()
        {
            this.InitializeComponent();
        }
    }
}
