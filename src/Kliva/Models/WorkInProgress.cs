using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Data;
using DynamicData;
using DynamicData.Binding;
using GalaSoft.MvvmLight.Threading;
using Kliva.Services.Interfaces;
using System.Reactive.Linq;
using DynamicData.Controllers;

namespace Kliva.Models
{
    // this is all currently work in progress hacking so be kind :) ....

    internal class ActivitySummaryCacheLoader
    {
        public delegate Task<string> LoadMethod(int page, int pageSize);

        private readonly LoadMethod _loader;
        private readonly Func<string, Task<List<ActivitySummary>>> _hydrater;
        private readonly int _pageSize;
        private int _page;
        private readonly ISourceCache<ActivitySummary, long> _targetCache;

        public ActivitySummaryCacheLoader(
            LoadMethod loader,
            Func<string, Task<List<ActivitySummary>>> hydrater,
            int pageSize, ISourceCache<ActivitySummary, long> targetCache)
        {
            if (loader == null) throw new ArgumentNullException(nameof(loader));
            if (hydrater == null) throw new ArgumentNullException(nameof(hydrater));
            if (targetCache == null) throw new ArgumentNullException(nameof(targetCache));

            _loader = loader;
            _hydrater = hydrater;
            _pageSize = pageSize;            
            _targetCache = targetCache;
        }

        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItems(uint count)
        {
            return AsyncInfo.Run(_ => LoadMoreItemsAsync());
        }

        public bool IsLoading { get; private set; }

        public bool LastLoadReturnedData { get; private set; }

        private async Task<LoadMoreItemsResult> LoadMoreItemsAsync()
        {
            IsLoading = true;
            try
            {
                var data = await _loader(_page + 1, _pageSize);
                if (string.IsNullOrEmpty(data)) return new LoadMoreItemsResult { Count = 0 };
                _page++;
                var items = await HydrateItems(data);

                _targetCache.AddOrUpdate(items);
                LastLoadReturnedData = items.Count > 0;

                return new LoadMoreItemsResult { Count = (uint)items.Count };
            }
            finally
            {
                IsLoading = false;
            }            
        }

        private async Task<List<ActivitySummary>> HydrateItems(string data)
        {
            IEnumerable<ActivitySummary> results = await _hydrater(data);            

            return results.ToList();
        }
    }

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
                friendsActivitySummaryCache);
            //..and just me. 
            var myCacheLoader = new ActivitySummaryCacheLoader(_stravaService.GetMyActivityDataAsync, _stravaService.HydrateActivityData, pageSize,
                myActivitySummaryCache);
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

    public static class DynamicDataEx
    {
        /// <summary>
        /// This is a local version of the standard DynamicData Bind extension, binding a <see cref="DeferringObservableCollection{T}"/>.
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="readOnlyObservableCollection"></param>
        /// <param name="loadMoreItems"></param>
        /// <param name="hasMoreItems"></param>
        /// <param name="resetThreshold"></param>
        /// <returns></returns>
        public static IObservable<IChangeSet<TObject, TKey>> Bind<TObject, TKey>(this IObservable<IChangeSet<TObject, TKey>> source, out DeferringObservableCollection<TObject> readOnlyObservableCollection, Func<uint, IAsyncOperation<LoadMoreItemsResult>> loadMoreItems, Func<bool> hasMoreItems, int resetThreshold = 30)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            var collectionExtended = new ObservableCollectionExtended<TObject>();
            var observableCollection = new DeferringObservableCollection<TObject>(collectionExtended, loadMoreItems, hasMoreItems);
            var collectionAdaptor = new ObservableCollectionAdaptor<TObject, TKey>(resetThreshold);
            readOnlyObservableCollection = observableCollection;            
            return source.Bind(collectionExtended, collectionAdaptor);
        }
    }

    public class DeferringObservableCollection<TContent> : ReadOnlyObservableCollection<TContent>, ISupportIncrementalLoading
    {
        private readonly Func<uint, IAsyncOperation<LoadMoreItemsResult>> _loadMoreItems;
        private readonly Func<bool> _hasMoreItems;

        public DeferringObservableCollection(ObservableCollection<TContent> list, Func<uint, IAsyncOperation<LoadMoreItemsResult>> loadMoreItems, Func<bool> hasMoreItems) : base(list)
        {
            _loadMoreItems = loadMoreItems;
            _hasMoreItems = hasMoreItems;
        }

        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count) => _loadMoreItems(count);

        public bool HasMoreItems => _hasMoreItems();
    }
}
