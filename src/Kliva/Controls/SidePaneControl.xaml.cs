using Windows.UI.Xaml.Controls;
using Kliva.ViewModels;

namespace Kliva.Controls
{
    public sealed partial class SidePaneControl : UserControl
    {
        private SidePaneViewModel ViewModel => DataContext as SidePaneViewModel;

        public SidePaneControl()
        {
            this.InitializeComponent();

            //Needed when using x:Bind in the SidePane control XAML
            DataContextChanged += (sender, args) => this.Bindings.Update();
        }
    }
}
