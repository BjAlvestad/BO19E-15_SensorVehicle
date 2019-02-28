using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading;

namespace Helpers
{
    public class ExclusiveSynchronizedObservableCollection<T> : ObservableCollection<T>
    {
        private SynchronizationContext _uiSynchronizationContext;
        public ExclusiveSynchronizedObservableCollection()
        {
            _uiSynchronizationContext = SynchronizationContext.Current;
        }

        public ExclusiveSynchronizedObservableCollection(SynchronizationContext uiSynchronizationContext)
        {
            _uiSynchronizationContext = uiSynchronizationContext;
        }

        public void AddFromArray(T[] items)
        {
            foreach (T item in items)
            {
                Add(item);
            }
        }

        protected override void InsertItem(int index, T item) 
        { 
            if (Contains(item)) return;  // Item already exists in collection. Return without adding.

            base.InsertItem(index, item); 
        } 

        protected override void SetItem(int index, T item) 
        { 
            int i = IndexOf(item); 
            if (i >= 0 && i != index) return;  // Item already exists in collection. Return without adding.

            base.SetItem(index, item); 
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (_uiSynchronizationContext == null)
            {
                base.OnCollectionChanged(e);
            }
            else
            {
                _uiSynchronizationContext.Post((_) => base.OnCollectionChanged(e), null);
            }
        }
    }
}
