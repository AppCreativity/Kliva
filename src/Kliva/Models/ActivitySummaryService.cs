using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using GalaSoft.MvvmLight.Threading;
using Kliva.Services.Interfaces;

namespace Kliva.Models
{
    public class ActivitySummaryService
    {
        private readonly IStravaService _stravaService;
        private readonly IStravaAthleteService _stravaAthleteService;        

        public ActivitySummaryService(IStravaService stravaService, IStravaAthleteService stravaAthleteService)
        {
            if (stravaService == null) throw new ArgumentNullException(nameof(stravaService));
            if (stravaAthleteService == null) throw new ArgumentNullException(nameof(stravaAthleteService));

            _stravaService = stravaService;
            _stravaAthleteService = stravaAthleteService;
        }
        
        public IDisposable Bind(            
            out DeferringObservableCollection<ActivitySummary> friendsCollection,
            out DeferringObservableCollection<ActivitySummary> myCollection,
            out DeferringObservableCollection<ActivitySummary> allCollection
            )
        {
            //could look at make these caches application wide, but for now scope to the subscription
            var friendsActivitySummaryCache = new SourceCache<ActivitySummary, long>(activitySummary => activitySummary.Id);
            var myActivitySummaryCache = new SourceCache<ActivitySummary, long>(activitySummary => activitySummary.Id);
            
            const int pageSize = 30;

            //we have two caches, which we use to load friends (followers and me)...
            var friendsCacheLoader = new ActivitySummaryCacheLoader(_stravaService.GetFriendActivityDataAsync, _stravaService.HydrateActivityData, pageSize,
                friendsActivitySummaryCache, "All");
            //..and just me. 
            var myCacheLoader = new ActivitySummaryCacheLoader(_stravaService.GetMyActivityDataAsync, _stravaService.HydrateActivityData, pageSize,
                myActivitySummaryCache, "My");
            //KEEP THIS ABOVE VARS...we can create a Reset() method, which will handle our pull to refresh

            friendsCacheLoader.LoadMoreItems(pageSize);
            myCacheLoader.LoadMoreItems(pageSize);

            var sortExpressionComparer = SortExpressionComparer<ActivitySummary>.Descending(summary => summary.DateTimeStartLocal);
            var allCollectionSubscription = friendsActivitySummaryCache.Connect()
                .Sort(sortExpressionComparer)
                .ObserveOn(DispatcherHelper.UIDispatcher)
                .Bind(out allCollection, friendsCacheLoader.LoadMoreItems, () => !friendsCacheLoader.IsLoading)
                .Subscribe();
            var friendsCollectionSubscription = friendsActivitySummaryCache.Connect()
                .Filter(activitySummary => activitySummary.Athlete.Id != _stravaAthleteService.Athlete.Id)
                .Sort(sortExpressionComparer)
                .ObserveOn(DispatcherHelper.UIDispatcher)
                .Bind(out friendsCollection, myCacheLoader.LoadMoreItems, () => !friendsCacheLoader.IsLoading)
                .Subscribe();
            var myCollectionSubscription = myActivitySummaryCache.Connect()
                .Sort(sortExpressionComparer)
                .ObserveOn(DispatcherHelper.UIDispatcher)
                .Bind(out myCollection, myCacheLoader.LoadMoreItems, () => !myCacheLoader.IsLoading)
                .Subscribe();

            return new CompositeDisposable(friendsActivitySummaryCache, myActivitySummaryCache,
                allCollectionSubscription, friendsCollectionSubscription, myCollectionSubscription);            
        }
    }
}