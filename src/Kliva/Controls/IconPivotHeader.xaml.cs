using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Kliva.Controls
{
    public sealed partial class IconPivotHeader : UserControl
    {
        public static readonly DependencyProperty GlyphProperty = DependencyProperty.Register("Glyph", typeof(string), typeof(IconPivotHeader), null);

        public string Glyph
        {
            get { return GetValue(GlyphProperty) as string; }
            set { SetValue(GlyphProperty, value); }
        }

        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register("Label", typeof(string), typeof(IconPivotHeader), null);

        public string Label
        {
            get { return GetValue(LabelProperty) as string; }
            set { SetValue(LabelProperty, value); }
        }

        public IconPivotHeader()
        {
            InitializeComponent();
            DataContext = this;
        }
    }
}
