using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kliva.Services.Interfaces;

namespace Kliva.Models
{
    public class ActivityIncrementalCollection : IncrementalLoadingBase
    {
        private IStravaService _stravaService;
        private IEnumerable<ActivitySummary> _activities;
        private bool _hasLoaded = false;
        private int _page = 1;

        public ActivityIncrementalCollection(IStravaService stravaService)
        {
            _stravaService = stravaService;
        }

        protected override async Task<IList<object>> LoadMoreItemsOverrideAsync(CancellationToken c, uint count)
        {
            _hasLoaded = true;
            _activities = await _stravaService.GetActivitiesWithAthletesAsync(_page, 30);

            if (_activities != null && _activities.Any())
            {
                ++_page;

                return new List<object>(_activities);
            }

            return null;
        }

        protected override bool HasMoreItemsOverride()
        {
            if(_hasLoaded)
                return _activities != null && _activities.Any();
            else
                return true;
        }
    }
}
