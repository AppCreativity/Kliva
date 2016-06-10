using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Controls;
using Kliva.Models;

namespace Kliva.Controls
{
    public class Gear
    {
        public string DisplayName { get; set; }
        public string GearID { get; set; }
    }

    public sealed partial class EditContentDialog : ContentDialog
    {
        public string ActivityName { get; private set; }
        public bool ActivityCommute { get; private set; }
        public bool ActivityPrivate { get; private set; }
        public Gear SelectedGear { get; set; }

        public ObservableCollection<Gear> GearList { get; set; } = new ObservableCollection<Gear>();

        public EditContentDialog(string activityName, bool activityCommute, bool activityPrivate, List<GearSummary> gear)
        {
            this.InitializeComponent();
            this.DataContext = this;

            foreach (GearSummary gearSummary in gear)
                GearList.Add(new Gear() { DisplayName = gearSummary.Name, GearID = gearSummary.Id });

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
