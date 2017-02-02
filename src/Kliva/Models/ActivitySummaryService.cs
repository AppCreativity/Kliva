using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using GalaSoft.MvvmLight.Threading;
using Kliva.Services.Interfaces;

namespace Kliva.Models
{
    public class ActivitySummaryService : IDisposable
    {        
        private readonly IStravaAthleteService _stravaAthleteService;
        private readonly SourceCache<ActivitySummary, long> _friendsActivitySummaryCache;
        private readonly SourceCache<ActivitySummary, long> _myActivitySummaryCache;
        private readonly ActivitySummaryCacheLoader _friendsCacheLoader;
        private readonly ActivitySummaryCacheLoader _myCacheLoader;
        private readonly CompositeDisposable _disposable;

        public ActivitySummaryService(IStravaService stravaService, IStravaAthleteService stravaAthleteService)
        {
            if (stravaService == null) throw new ArgumentNullException(nameof(stravaService));
            if (stravaAthleteService == null) throw new ArgumentNullException(nameof(stravaAthleteService));

            _stravaAthleteService = stravaAthleteService;

            _friendsActivitySummaryCache = new SourceCache<ActivitySummary, long>(activitySummary => activitySummary.Id);
            _myActivitySummaryCache = new SourceCache<ActivitySummary, long>(activitySummary => activitySummary.Id);

            const int pageSize = 30;

            //we have two caches, which we use to load friends (followers and me)...
            _friendsCacheLoader = new ActivitySummaryCacheLoader(stravaService.GetFriendActivityDataAsync, stravaService.HydrateActivityData, pageSize,
                _friendsActivitySummaryCache, "All");
            //..and just me. 
            _myCacheLoader = new ActivitySummaryCacheLoader(stravaService.GetMyActivityDataAsync, stravaService.HydrateActivityData, pageSize,
                _myActivitySummaryCache, "My");

            _friendsCacheLoader.LoadMoreItems(pageSize);
            _myCacheLoader.LoadMoreItems(pageSize);

            _disposable = new CompositeDisposable(_friendsActivitySummaryCache, _myActivitySummaryCache);
        }
        
        public IDisposable Bind(            
            out DeferringObservableCollection<ActivitySummary> friendsCollection,
            out DeferringObservableCollection<ActivitySummary> myCollection,
            out DeferringObservableCollection<ActivitySummary> allCollection
            )
        {                        
            var sortExpressionComparer = SortExpressionComparer<ActivitySummary>.Descending(summary => summary.DateTimeStartLocal);
            var allCollectionSubscription = _friendsActivitySummaryCache.Connect()
                .Sort(sortExpressionComparer)
                .ObserveOn(DispatcherHelper.UIDispatcher)
                .Bind(out allCollection, _friendsCacheLoader.LoadMoreItems, () => !_friendsCacheLoader.IsLoading)
                .Subscribe();
            var friendsCollectionSubscription = _friendsActivitySummaryCache.Connect()
                .Filter(activitySummary => activitySummary.Athlete.Id != _stravaAthleteService.Athlete.Id)
                .Sort(sortExpressionComparer)
                .ObserveOn(DispatcherHelper.UIDispatcher)
                .Bind(out friendsCollection, _myCacheLoader.LoadMoreItems, () => !_friendsCacheLoader.IsLoading)
                .Subscribe();
            var myCollectionSubscription = _myActivitySummaryCache.Connect()
                .Sort(sortExpressionComparer)
                .ObserveOn(DispatcherHelper.UIDispatcher)
                .Bind(out myCollection, _myCacheLoader.LoadMoreItems, () => !_myCacheLoader.IsLoading)
                .Subscribe();

            return new CompositeDisposable(allCollectionSubscription, friendsCollectionSubscription, myCollectionSubscription);            
        }

        public void Refresh()
        {
            _friendsCacheLoader.Reset();
            _myCacheLoader.Reset();
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }
    }
}