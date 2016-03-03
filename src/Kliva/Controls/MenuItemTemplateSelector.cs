using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Kliva.Models;

namespace Kliva.Controls
{
    public class MenuItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate MDL2DataTemplate { get; set; }
        public DataTemplate MaterialDataTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            var listType = item as MenuItem;

            if (listType == null)
                return base.SelectTemplateCore(item);

            switch (listType.MenuItemFontType)
            {
                case MenuItemFontType.MDL2:
                    return MDL2DataTemplate;
                case MenuItemFontType.Material:
                    return MaterialDataTemplate;
            }

            return MDL2DataTemplate;
        }
    }
}
