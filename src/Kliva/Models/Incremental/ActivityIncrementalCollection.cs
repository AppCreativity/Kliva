using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kliva.Services.Interfaces;
using Kliva.Helpers;

namespace Kliva.Models
{
    public abstract class ActivityIncrementalCollection : CachedKeyedIncrementalLoadingBase
    {
        private readonly IStravaService _stravaService;

        protected ActivityIncrementalCollection(IStravaService stravaService, ActivityFeedFilter cachename)
            : base(cachename)
        {
            _stravaService = stravaService;
        }

        protected override async Task<List<object>> HydrateItems(string data)
        {
            var results = await _stravaService.HydrateActivityData(data);
            //foreach( var item in from r in results where r.PhotoCount > 0 select r) // TODO review
            //  {

            //  }
            return results.Cast<object>().ToList();
        }
    }
}
