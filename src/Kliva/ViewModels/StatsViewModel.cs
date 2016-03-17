using System;
using System.Collections.Generic;
using System.Linq;
using Cimbalino.Toolkit.Services;
using Kliva.Helpers;
using Kliva.Models;
using Kliva.Services.Interfaces;

namespace Kliva.ViewModels
{
    public class DayStats
    {
        public DateTime Day { get; set;}
        public float Distance { get; set; }
    }

    public class StatsViewModel : KlivaBaseViewModel
    {
        private readonly DateTime _firstDayOfWeek = DateTime.Now.GetFirstDayOfTheWeek();
        private readonly MyActivityIncrementalCollection _myActivityIncrementalCollection;

        public StatsViewModel(INavigationService navigationService, IStravaService stravaService) : base(navigationService)
        {
            _myActivityIncrementalCollection = new MyActivityIncrementalCollection(stravaService);
            _myActivityIncrementalCollection.DataLoaded += OnDataLoaded;
        }

        private async void OnDataLoaded(object sender, EventArgs e)
        {
            //TODO: Glenn - see 'how' long we need to hold the DataLoaded event
            //TODO: Glenn - how and when do we need to refresh the stats page?
            _myActivityIncrementalCollection.DataLoaded -= OnDataLoaded;

            //Check if we have enough data to show the 'week' graph
            if (_myActivityIncrementalCollection.Count > 0)
            {
                ActivitySummary lastActivitySummary = (ActivitySummary)_myActivityIncrementalCollection[_myActivityIncrementalCollection.Count - 1];

                //When the user has a lot of activities in 1 week, could be we still need to retrieve more data!
                //TODO: Glenn - Validate if HasMoreItems is actually being set correctly?
                if (lastActivitySummary.DateTimeStart >= _firstDayOfWeek && _myActivityIncrementalCollection.HasMoreItems)
                {
                    _myActivityIncrementalCollection.DataLoaded += OnDataLoaded;
                    await _myActivityIncrementalCollection.LoadMoreItemsAsync(0);
                }
                else
                {
                    //TODO: Glenn - Do we need to use DateTimeLocal?
                    List<DayStats> dayStatistics = (from ActivitySummary item in _myActivityIncrementalCollection
                                                    where item.DateTimeStart >= _firstDayOfWeek
                                                    group item by item.DateTimeStart.Date
                                                    into g
                                                    select new DayStats() {Day = g.Key, Distance = g.Sum(item => item.Distance)}).ToList();

                    //Now we should have summed up data grouped per day of the week
                }
            }
        }
    }
}
