using Kliva.ViewModels;
using Kliva.ViewModels.Interfaces;
using Windows.UI.Xaml.Controls;

namespace Kliva.Controls
{
    public sealed partial class ActivityFeedControl : UserControl
    {
        private IStravaViewModel ViewModel => DataContext as IStravaViewModel;

        public ActivityFeedControl()
        {
            this.InitializeComponent();
            DataContextChanged += (sender, args) => this.Bindings.Update();
        }        
    }
}
