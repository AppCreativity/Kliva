using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Text;
using System.Threading;
using Windows.Foundation;
using Windows.UI.Xaml.Data;
using DynamicData.Controllers;

namespace Kliva.Models
{
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
