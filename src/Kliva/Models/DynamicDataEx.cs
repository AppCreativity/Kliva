using System;
using Windows.Foundation;
using Windows.UI.Xaml.Data;
using DynamicData;
using DynamicData.Binding;

namespace Kliva.Models
{
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
}