using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Kliva.Helpers
{
    public class Grouping<K, T> : ObservableCollection<T>, IGrouping<K, T>
    {
        public K Key { get; private set; }

        public Grouping(K key, IEnumerable<T> items)
        {
            Key = key;
            foreach (var item in items)
                this.Items.Add(item);
        }
    }
}
