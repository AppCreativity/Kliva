using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kliva.Services;
using Kliva.Services.Interfaces;
using Microsoft.Practices.ServiceLocation;

namespace Kliva.Models
{
    public abstract class ActivityIncrementalCollection : CachedKeyedIncrementalLoadingBase
    {
        private readonly IStravaService _stravaService;
        private readonly IStravaAthleteService _athleteService;
        private readonly ISettingsService _settingsService;

        protected ActivityIncrementalCollection(IStravaService stravaService, ActivityFeedFilter cachename)
            : base(cachename)
        {
            _stravaService = stravaService;
            _athleteService = ServiceLocator.Current.GetInstance<IStravaAthleteService>();
            _settingsService = ServiceLocator.Current.GetInstance<ISettingsService>();
        }

        protected override async Task<List<object>> HydrateItems(string data)
        {
            IEnumerable<ActivitySummary> results = await _stravaService.HydrateActivityData(data);

            if(_filter == ActivityFeedFilter.Friends)
                results = results.Where(activity => activity.Athlete.Id != _athleteService.Athlete.Id);

            ActivitySort sort = await _settingsService.GetStoredActivitySortAsync();
            if (sort == ActivitySort.EndTime)
                results = results.OrderBy(activity => activity.EndDate);

            return results.Cast<object>().ToList();
        }
    }
}
