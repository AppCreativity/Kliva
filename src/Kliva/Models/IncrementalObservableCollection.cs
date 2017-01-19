using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Data;
using DynamicData;

namespace Kliva.Models
{
    // this is all currently work in progress hacking so be kind :) ....

    
    public class ActivitySummaryCache
    {
        private readonly ISourceCache<ActivitySummary, long> _sourceCache = new SourceCache<ActivitySummary, long>(activitySummary => activitySummary.Id);

        public ActivitySummaryCache()
        {
            //_sourceCache.
        }

        public IObservable<IChangeSet<ActivitySummary, long>> Connect()
        {
            return _sourceCache.Connect();
        }
    }


    class Hack
    {
        public void X()
        {
            ReadOnlyObservableCollection<ActivitySummary> col;
            var disposeMe = new ActivitySummaryCache().Connect().Bind(out col).Subscribe();

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
