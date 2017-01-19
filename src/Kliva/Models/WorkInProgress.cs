using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

namespace Kliva.Models
{
    // this is all currently work in progress hacking so be kind :) ....

    public class ActivitySummaryService
    {
        private readonly IStravaService _stravaService;
        private readonly ISourceCache<ActivitySummary, long> _sourceCache = new SourceCache<ActivitySummary, long>(activitySummary => activitySummary.Id);
        private int _page;
        private int _pageSize = 30;

        public ActivitySummaryService(IStravaService stravaService, IStravaAthleteService stravaAthleteService)
        {
            if (stravaService == null) throw new ArgumentNullException(nameof(stravaService));

            _stravaService = stravaService;            
        }

        //TODO ActivityFeedfiltering : Filter (MainViewModel.ApplyActivityFeedFilter)
        public IDisposable Bind(
            /*IObservable<Func<ActivityFeedFilter>> filter,*/ out DeferringObservableCollection<ActivitySummary> collection)
        {   
                     

            return _sourceCache.Connect()
                //.Filter(filter.Select())
                .Bind(out collection, LoadMoreItems, HasMoreItems).Subscribe();

        }

        //TODO previos version was checking initial load first I think
        private bool HasMoreItems()
        {
            return true;
        }

        private IAsyncOperation<LoadMoreItemsResult> LoadMoreItems(uint count)
        {
            /* TODO ...I dont know if we need this?
            if (_busy)
            {
                throw new InvalidOperationException("Only one operation in flight at a time");
            }

            _busy = true;
            */

            return AsyncInfo.Run(_ => LoadMoreItemsAsync());
        }

        private async Task<LoadMoreItemsResult> LoadMoreItemsAsync()
        {
            try
            {
                var data = await FetchData(_page + 1, _pageSize);
                if (data == null) return new LoadMoreItemsResult {Count = 0};
                _page++;
                var items = await HydrateItems(data);

                _sourceCache.AddOrUpdate(items);

                /*DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        MergeInItems(items);
                        HasData = (items.Count > 1);
                    });
                    */

                return new LoadMoreItemsResult { Count = (uint)items.Count };                
            }
            finally
            {
                //TODO confirm usage?
                //_busy = false;
            }
        }

        private async Task<List<ActivitySummary>> HydrateItems(string data)
        {
            IEnumerable<ActivitySummary> results = await _stravaService.HydrateActivityData(data);

            //TODO: filtering
            /*
            if (_filter == ActivityFeedFilter.Friends)
                results = results.Where(activity => activity.Athlete.Id != _athleteService.Athlete.Id);
                */

            //TODO: local cache, (on startup?)
            /*
            ActivitySort sort = await _settingsService.GetStoredActivitySortAsync();
            if (sort == ActivitySort.EndTime)
                results = results.OrderByDescending(activity => activity.EndDate);
                */

            return results.ToList();
        }


        //TODO...act according to filter?
        private Task<string> FetchData(int page, int pageSize)
        {
            return _stravaService.GetFriendActivityDataAsync(page, pageSize);
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
        public static IObservable<IChangeSet<TObject, TKey>> Bind<TObject, TKey>(
            this IObservable<IChangeSet<TObject, TKey>> source, 
            out DeferringObservableCollection<TObject> readOnlyObservableCollection,
            Func<uint, IAsyncOperation<LoadMoreItemsResult>> loadMoreItems,
            Func<bool> hasMoreItems,
            int resetThreshold = 30)
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

        public DeferringObservableCollection(ObservableCollection<TContent> list, 
            Func<uint, IAsyncOperation<LoadMoreItemsResult>> loadMoreItems, 
            Func<bool> hasMoreItems 
            ) : base(list)
        {
            _loadMoreItems = loadMoreItems;
            _hasMoreItems = hasMoreItems;
        }

        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count) => _loadMoreItems(count);

        public bool HasMoreItems => _hasMoreItems();
    }
}
