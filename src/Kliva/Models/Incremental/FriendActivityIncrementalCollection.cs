using System.Threading.Tasks;
using Kliva.Services.Interfaces;

namespace Kliva.Models
{
    public class FriendActivityIncrementalCollection : ActivityIncrementalCollection
    {
        private readonly IStravaService _stravaService;

        public FriendActivityIncrementalCollection(IStravaService stravaService, ActivityFeedFilter filter)
            : base(stravaService, filter)
        {
            _stravaService = stravaService;
        }

        protected override Task<string> FetchData(int page, int pageSize)
        {
            return _stravaService.GetFriendActivityDataAsync(page, pageSize);
        }
    }
}