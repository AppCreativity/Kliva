using Windows.UI.Xaml.Controls;

namespace Kliva.Controls
{
    public sealed partial class SidePaneControl : UserControl
    {
        public SidePaneControl()
        {
            this.InitializeComponent();
            //TODO: Glenn - Uncomment when using x:Bind in the SidePane control XAML
            //DataContextChanged += (sender, args) => this.Bindings.Update();
        }
    }
}
