using Windows.UI.Xaml.Controls;

namespace Kliva.Controls
{
    public sealed partial class EditContentDialog : ContentDialog
    {
        public string ActivityName { get; private set; }
        public bool ActivityCommute { get; private set; }

        public EditContentDialog(string activityName, bool activityCommute)
        {
            this.InitializeComponent();
            this.DataContext = this;

            ActivityName = activityName;
            ActivityCommute = activityCommute;
        }

        private void OnContentDialogPrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            ActivityName = ActivityNameTextBox.Text;
            ActivityCommute = ActivityCommuteToggle.IsOn;
        }

        private void OnContentDialogSecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }
}
