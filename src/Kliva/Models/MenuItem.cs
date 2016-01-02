namespace Kliva.Models
{
    public class MenuItem : BaseClass
    {
        private string _icon;
        public string Icon
        {
            get { return _icon; }
            set { Set(() => Icon, ref _icon, value); }
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set { Set(() => Title, ref _title, value); }
        }
    }
}
