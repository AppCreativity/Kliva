using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Kliva.ViewModels.Interfaces;

namespace Kliva.Controls
{
    public sealed partial class ActivityDetailControl : UserControl
    {
        public IStravaViewModel ViewModel => DataContext as IStravaViewModel;

        public ActivityDetailControl()
        {
            this.InitializeComponent();
            //DataContextChanged += (sender, arg) => this.Bindings.Update();
        }             
    }
}
