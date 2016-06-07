using Windows.UI.Xaml.Controls;

namespace Kliva.Controls
{
    public sealed partial class EditContentDialog : ContentDialog
    {
        public string ActivityName { get; private set; }

        public EditContentDialog(string activityName)
        {
            this.InitializeComponent();
            this.DataContext = this;

            ActivityName = activityName;
        }

        private void OnContentDialogPrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void OnContentDialogSecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }
}
