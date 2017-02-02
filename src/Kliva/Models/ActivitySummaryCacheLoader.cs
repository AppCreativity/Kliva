using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Data;
using DynamicData;
using Kliva.Services;

namespace Kliva.Models
{
    internal class ActivitySummaryCacheLoader
    {
        public delegate Task<string> LoadMethod(int page, int pageSize);

        private readonly LoadMethod _loader;
        private readonly Func<string, Task<List<ActivitySummary>>> _hydrater;
        private readonly int _pageSize;
        private int _page;
        private readonly ISourceCache<ActivitySummary, long> _targetCache;
        private readonly string _backingStoreCacheName;

        public ActivitySummaryCacheLoader(
            LoadMethod loader,
            Func<string, Task<List<ActivitySummary>>> hydrater,
            int pageSize, ISourceCache<ActivitySummary, long> targetCache,
            string backingStoreCacheName)
        {
            if (loader == null) throw new ArgumentNullException(nameof(loader));
            if (hydrater == null) throw new ArgumentNullException(nameof(hydrater));
            if (targetCache == null) throw new ArgumentNullException(nameof(targetCache));
            if (backingStoreCacheName == null) throw new ArgumentNullException(nameof(backingStoreCacheName));

            _loader = loader;
            _hydrater = hydrater;
            _pageSize = pageSize;            
            _targetCache = targetCache;
            _backingStoreCacheName = backingStoreCacheName;
        }

        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItems(uint count)
        {
            return AsyncInfo.Run(_ => LoadMoreItemsAsync());
        }

        public bool IsLoading { get; private set; }        

        private async Task<LoadMoreItemsResult> LoadMoreItemsAsync()
        {
            IsLoading = true;
            try
            {
                //grab from local cache to get some data quick
                if (_page == 0)
                {
                    var localCacheData = await LocalCacheService.ReadCacheData(_backingStoreCacheName);
                    if (localCacheData != null && localCacheData.Length > 5)
                    {
                        var cachedItems = await HydrateItems(localCacheData);
                        _targetCache.AddOrUpdate(cachedItems);
                    }
                }

                //hit the strava api
                var data = await _loader(_page + 1, _pageSize);
                if (string.IsNullOrEmpty(data)) return new LoadMoreItemsResult { Count = 0 };
                _page++;
                var items = await HydrateItems(data);
                _targetCache.AddOrUpdate(items);

                //cache the first page for next time
                //TODO check OK when implement refresh
                if (_page == 1)
                {
                    LocalCacheService.PersistCacheData(data, _backingStoreCacheName);
                }

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
}