using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight.Messaging;
using Kliva.Helpers;
using Kliva.Messages;
using Kliva.Models;
using Kliva.ViewModels;
using Microsoft.Practices.ServiceLocation;

namespace Kliva.Views
{
    public sealed partial class ProfilePage : Page
    {
        private ProfileViewModel ViewModel => DataContext as ProfileViewModel;

        //private readonly Dictionary<Pivots, Tuple<int, PivotItem>> _pivotDictionary = new Dictionary<Pivots, Tuple<int, PivotItem>>();

        public ProfilePage()
        {
            this.InitializeComponent();

            //TODO: Glenn - Set correct target!
            //ServiceLocator.Current.GetInstance<IMessenger>().Register<PivotMessage>(this, Tokens.ProfilePivotMessage, AdjustPivots);
        }

        private void OnProfilePageLoaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
