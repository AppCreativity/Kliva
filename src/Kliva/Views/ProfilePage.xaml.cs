using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight.Messaging;
using Kliva.Extensions;
using Kliva.Messages;
using Kliva.Models;
using Kliva.ViewModels;
using Microsoft.Practices.ServiceLocation;

namespace Kliva.Views
{
    public sealed partial class ProfilePage : Page
    {
        private ProfileViewModel ViewModel => DataContext as ProfileViewModel;

        private readonly Dictionary<ProfilePivots, Tuple<int, PivotItem>> _pivotDictionary = new Dictionary<ProfilePivots, Tuple<int, PivotItem>>();


        public ProfilePage()
        {
            InitializeComponent();

            ServiceLocator.Current.GetInstance<IMessenger>().Register<PivotMessage<ProfilePivots>>(this, Tokens.ProfilePivotMessage, AdjustPivots);
        }

        private void OnProfilePageLoaded(object sender, RoutedEventArgs e)
        {
            if (_pivotDictionary.Count == 0)
                IndexPivotCollection();
        }

        private void AdjustPivots(PivotMessage<ProfilePivots> message)
        {
            foreach (PivotItem item in ProfilePivot.Items.ToList())
            {
                if (item.Visibility == Visibility.Collapsed)
                    ProfilePivot.Items.Remove(item);
            }

            if (!ReferenceEquals(message, null) && message.Visible)
            {
                //Handle Defer Loaded pivots
                if (!_pivotDictionary.ContainsKey(message.Pivot))
                {
                    //Realize UI element
                    FindName($"{message.Pivot.ToString()}Pivot");
                    //Reindex collection
                    _pivotDictionary.Clear();
                    IndexPivotCollection();
                }
                else
                {
                    Tuple<int, PivotItem> pivotItem = _pivotDictionary[message.Pivot];

                    if (!ProfilePivot.Items.Contains(pivotItem.Item2))
                        ProfilePivot.Items.Insert(pivotItem.Item1, pivotItem.Item2);
                }
            }

            if (message.Show.HasValue && message.Show.Value)
                ProfilePivot.SelectedIndex = ProfilePivot.Items.IndexOf(_pivotDictionary[message.Pivot].Item2);
        }

        private void IndexPivotCollection()
        {
            int pivotIndex = 0;
            foreach (PivotItem item in ProfilePivot.Items.ToList())
            {
                _pivotDictionary.Add(Enum<ProfilePivots>.Parse((string)item.Tag), Tuple.Create(pivotIndex, item));
                ++pivotIndex;
            }
        }
    }
}
