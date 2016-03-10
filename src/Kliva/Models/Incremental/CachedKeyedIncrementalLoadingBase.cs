//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using GalaSoft.MvvmLight.Threading;
using Kliva.Services;

namespace Kliva.Models
{
    // This class can used as a jumpstart for implementing ISupportIncrementalLoading. 
    // Implementing the ISupportIncrementalLoading interfaces allows you to create a list that loads
    //  more data automatically when the user scrolls to the end of of a GridView or ListView.

    public abstract class CachedKeyedIncrementalLoadingBase : IList, ISupportIncrementalLoading, INotifyCollectionChanged
    {
        private bool _hasLoaded = false;
        //private bool _moreData = false;
        private int _page = 1;
        private int _pageSize = 30;
        protected bool HasData = false;

        // state
        private readonly Dictionary<long, object> _storageLookup = new Dictionary<long, object>();
        private readonly List<object> _storage = new List<object>();
        private bool _busy = false;
        private readonly string _name;
        protected ActivityFeedFilter _filter;


        #region Event handlers
        public event EventHandler DataLoaded;

        protected virtual void OnDataLoaded()
        {
            EventHandler handler = DataLoaded;
            handler?.Invoke(this, null);
        }
        #endregion

        protected CachedKeyedIncrementalLoadingBase(ActivityFeedFilter name)
        {
            _filter = name;

            //Currently the Strava API has no difference in returning Friends' feed of All feed, so we store the cache under the same name!
            switch (name)
            {
                case ActivityFeedFilter.All:
                case ActivityFeedFilter.Friends:
                    _name = ActivityFeedFilter.All.ToString();
                    break;
                default:
                    _name = name.ToString();
                    break;
            }            
            LoadNewData();
        }

        private void LoadNewData()
        {
            DateTime timestamp = DateTime.MinValue;
            int page = 1;

            Task t = new Task(async () =>
            {
                timestamp = await LocalCacheService.GetCacheTimestamp(_name);

                if (!_hasLoaded)
                {
                    string data = await LocalCacheService.ReadCacheData(this._name);
                    if (data != null && data.Length > 5)
                    {
                        var items = await HydrateItems(data);
                        MergeInItems(items);                        
                    }
                }

                if (DateTime.Now - timestamp > new TimeSpan(0, 5, 0))
                {
                    string data = await FetchData(page, _pageSize);
                    var items = await HydrateItems(data);
                    if (items != null && items.Count > 1)
                    {
                        LocalCacheService.PersistCacheData(data, this._name);
                    }
                    MergeInItems(items);
                }

                _hasLoaded = true;
                HasData = true;

                OnDataLoaded();
            });
            t.Start();
        }

        private void MergeInItems(List<object> items)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                var baseIndex = _storage.Count;
                int index;
                long firstkey = (_storage.Count > 0) ? ((IKey)(_storage[0])).Key : 0;
                int firstpos = 0;

                if (items != null)
                {
                    foreach (object newitem in items)
                    {
                        IKey keyedItem = newitem as IKey;
                        if (keyedItem != null)
                        {
                            object olditem = null;
                            long key = keyedItem.Key;
                            if (_storageLookup.TryGetValue(key, out olditem))
                            {
                                index = _storage.IndexOf(olditem);
                                _storage[index] = newitem;
                                _storageLookup[key] = newitem;
                                NotifyReplace(newitem, olditem, index);
                            }
                            else
                            {
                                if (key > firstkey)
                                {
                                    _storage.Insert(firstpos, newitem);
                                    _storageLookup.Add(key, newitem);
                                    NotifyInsert(newitem, firstpos);
                                    firstpos++;
                                }
                                else
                                {
                                    _storage.Add(newitem);
                                    _storageLookup.Add(key, newitem);
                                    NotifyInsert(newitem, _storage.Count - 1);
                                }
                            }
                        }
                    }
                }
            });
        }

        private void NotifyReplace(object newitem, object olditem, int index)
        {
            if (CollectionChanged != null)
            {
                var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newitem, olditem, index);
                CollectionChanged(this, args);
            }
        }

        void NotifyInsert(object item, int index)
        {
            if (CollectionChanged != null)
            {
                var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index);
                CollectionChanged(this, args);
            }
        }

        #region IList

        public int Add(object value)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(object value)
        {
            return _storage.Contains(value);
        }

        public int IndexOf(object value)
        {
            return _storage.IndexOf(value);
        }

        public void Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        public bool IsFixedSize
        {
            get { return false; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public void Remove(object value)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public object this[int index]
        {
            get
            {
                return _storage[index];
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void CopyTo(Array array, int index)
        {
            ((IList)_storage).CopyTo(array, index);
        }

        public int Count
        {
            get { return _storage.Count; }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public object SyncRoot
        {
            get { throw new NotSupportedException(); }
        }

        public IEnumerator GetEnumerator()
        {
            return _storage.GetEnumerator();
        }

        #endregion

        #region ISupportIncrementalLoading

        public bool HasMoreItems => HasMoreItemsOverride();

        public Windows.Foundation.IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            if (_busy)
            {
                throw new InvalidOperationException("Only one operation in flight at a time");
            }

            _busy = true;

            return AsyncInfo.Run((c) => LoadMoreItemsAsync(c, count));
        }

        #endregion 

        #region INotifyCollectionChanged

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion 

        #region Private methods

        async Task<LoadMoreItemsResult> LoadMoreItemsAsync(CancellationToken c, uint count)
        {
            try
            {
                string data = await FetchData(_page + 1, _pageSize);
                if (data != null)
                {
                    _page++;
                    var items = await HydrateItems(data);
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        MergeInItems(items);
                        HasData = (items.Count > 1);
                    });

                    return new LoadMoreItemsResult { Count = (uint)items.Count };
                }
                return new LoadMoreItemsResult() { Count = 0 };
            }
            finally
            {
                _busy = false;
            }
        }



        #endregion

        #region Overridable methods

        protected abstract Task<string> FetchData(int page, int pageSize);
        protected abstract Task<List<object>> HydrateItems(string data);
        protected bool HasMoreItemsOverride()
        {

            return (_hasLoaded && HasData);
        }


        #endregion 
    }
}