using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Kliva.Controls
{
    public sealed partial class LoadingControl : UserControl
    {
        public static readonly DependencyProperty IsLoadingProperty = DependencyProperty.Register(nameof(IsLoading), typeof(bool), typeof(LoadingControl), new PropertyMetadata(false));

        public bool IsLoading
        {
            get { return (bool)GetValue(IsLoadingProperty); }
            set { SetValue(IsLoadingProperty, value); }
        }

        public LoadingControl()
        {
            InitializeComponent();
        }

        public static void SetLoading(bool isLoading)
        {
            var action = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    var window = Window.Current.Content as KlivaApplicationFrame;
                    window?.ShowLoading(isLoading);
                });
        }
    }
}
