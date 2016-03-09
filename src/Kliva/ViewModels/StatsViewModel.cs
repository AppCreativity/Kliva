using System;
using System.Threading.Tasks;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using Kliva.Models;
using Kliva.Services.Interfaces;

namespace Kliva.ViewModels
{
    public class StatsViewModel : KlivaBaseViewModel
    {
        private readonly IStravaService _stravaService;

        public MyActivityIncrementalCollection ActivityIncrementalCollection { get; set; }

        public StatsViewModel(INavigationService navigationService, IStravaService stravaService) : base(navigationService)
        {
            _stravaService = stravaService;

            //TODO: Glenn - How can we 'mis'use the MyActivityIncrementalCollection to retrieve the current week stats?
            //TODO: Glenn - When the collection hasn't been loaded yet on the main page, this will contain nothing and start loading... but how do we know it's finished loading?
            //TODO: Glenn - The collectionChanged event will be triggered with each addition of an item, so currently we don't have a 'finished loading' event
            //ActivityIncrementalCollection = new MyActivityIncrementalCollection(_stravaService);
            //ActivityIncrementalCollection.CollectionChanged += OnActivityIncrementalCollectionCollectionChanged;
        }

        private void OnActivityIncrementalCollectionCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //if (ActivityIncrementalCollection.Count > 0)
            //{
            //    foreach (var VARIABLE in ActivityIncrementalCollection)
            //    {
            //        string t = "";
            //    }
            //    ActivityIncrementalCollection.CollectionChanged -= OnActivityIncrementalCollectionCollectionChanged;
            //}
        }
    }
}
