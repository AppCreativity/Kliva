using Windows.UI.Xaml.Controls;

namespace Kliva.Controls
{
    public sealed partial class CommentContentDialog : ContentDialog
    {
        public string Description { get; private set; } = string.Empty;

        public CommentContentDialog()
        {
            this.InitializeComponent();
        }

        private void OnContentDialogPrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Description = DescriptionTextBox.Text;
        }

        private void OnContentDialogSecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {            
        }
    }
}
