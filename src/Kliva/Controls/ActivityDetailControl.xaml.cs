using Windows.UI.Xaml.Controls;
using Kliva.ViewModels.Interfaces;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

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
