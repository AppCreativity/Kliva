using Windows.UI.Xaml.Controls;
using Kliva.ViewModels;

namespace Kliva.Controls
{
    public sealed partial class SidePaneControl : UserControl
    {
        private bool _listViewChanging = false;

        private SidePaneViewModel ViewModel => DataContext as SidePaneViewModel;

        public SidePaneControl()
        {
            this.InitializeComponent();

            //Needed when using x:Bind in the SidePane control XAML
            DataContextChanged += (sender, args) => this.Bindings.Update();
        }

        private void OnBottomMenuSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_listViewChanging)
            {
                _listViewChanging = true;
                TopMenu.SelectedIndex = -1;
                _listViewChanging = false;
            }
        }

        private void OnTopMenuSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_listViewChanging)
            {
                _listViewChanging = true;
                BottomMenu.SelectedIndex = -1;
                _listViewChanging = false;
            }            
        }
    }
}
