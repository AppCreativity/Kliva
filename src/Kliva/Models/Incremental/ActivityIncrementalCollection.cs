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

        protected ActivityIncrementalCollection(IStravaService stravaService, ActivityFeedFilter cachename)
            : base(cachename)
        {
            _stravaService = stravaService;
            _athleteService = ServiceLocator.Current.GetInstance<IStravaAthleteService>();
        }

        protected override async Task<List<object>> HydrateItems(string data)
        {
            var results = await _stravaService.HydrateActivityData(data);

            if(_filter == ActivityFeedFilter.Friends)
                results = results.Where(activity => activity.Athlete.Id != _athleteService.Athlete.Id).ToList();

            //foreach( var item in from r in results where r.PhotoCount > 0 select r) // TODO review
            //  {

            //  }

            return results.Cast<object>().ToList();
        }
    }
}
