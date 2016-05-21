using System.Threading.Tasks;
using Kliva.Services.Interfaces;

namespace Kliva.Models
{
    public class MyActivityIncrementalCollection : ActivityIncrementalCollection
    {
        private readonly IStravaService _stravaService;

        public MyActivityIncrementalCollection(IStravaService stravaService) : base(stravaService, ActivityFeedFilter.My)
        {
            _stravaService = stravaService;
        }

        protected override Task<string> FetchData(int page, int pageSize)
        {
            return _stravaService.GetMyActivityDataAsync(page, pageSize);
        }
    }
}