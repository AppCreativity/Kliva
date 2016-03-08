using System.Diagnostics.Tracing;

namespace Kliva.Services.Performance
{
    internal class ETWLogging : EventSource
    {
        public static ETWLogging Log = new ETWLogging(); // TODO replace static by singleton in IoC

        public void GetPhotosFromService(bool finished, string id) { WriteEvent(1, finished, id); }
        public void GetRelatedActivitiesFromService(bool finished, string id) { WriteEvent(2, finished, id); }
        public void GetActivityAsync(bool finished, string id, bool includeEfforts) { WriteEvent(3, finished, id, includeEfforts); }
        public void GetActivitiesAsync(bool finished, int page, int perPage) { WriteEvent(4, finished, page, perPage); }
        public void GetFollowersActivitiesAsync(bool finished, int page, int perPage) { WriteEvent(5, finished, page, perPage); }
        public void GetKudosAsync(bool finished, string id) { WriteEvent(6, finished, id); }

        public void GetAthleteFromServiceAsync(bool finished, string id) { WriteEvent(7, finished, id); }
        public void GetAthleteAsync(bool finished) { WriteEvent(8, finished); }
        public void GetFriendActivityDataAsync(bool finished, int page, int perPage) { WriteEvent(9, finished, page, perPage); }
    }
}