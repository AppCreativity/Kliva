using Windows.UI.Xaml.Controls;

namespace Kliva.Controls
{
    public sealed partial class EditContentDialog : ContentDialog
    {
        public string ActivityName { get; private set; }
        public bool ActivityCommute { get; private set; }
        public bool ActivityPrivate { get; private set; }

        public EditContentDialog(string activityName, bool activityCommute, bool activityPrivate)
        {
            this.InitializeComponent();
            this.DataContext = this;

            ActivityName = activityName;
            ActivityCommute = activityCommute;
            ActivityPrivate = activityPrivate;
        }

        private void OnContentDialogPrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            ActivityName = ActivityNameTextBox.Text;
            ActivityCommute = ActivityCommuteToggle.IsOn;
            ActivityPrivate = ActivityPrivateToggle.IsOn;
        }

        private void OnContentDialogSecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }
}
