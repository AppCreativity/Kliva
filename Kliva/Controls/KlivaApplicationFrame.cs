using Kliva.ViewModels;
using Kliva.Views;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

namespace Kliva.Controls
{
    public class KlivaApplicationFrame : Frame
    {
        private readonly List<Type> _noSidePane = new List<Type>
        {
            typeof(LoginPage),
            typeof(SettingsPage)
        };

        public KlivaApplicationFrame()
        {
            Navigated += OnNavigated;
        }

        private void OnNavigated(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            var pageType = e.Content.GetType();
            var show = !_noSidePane.Contains(pageType);

            ViewModelLocator.Get<SidePaneViewModel>().ShowHide(show);
        }
    }
}
