using System.Collections.ObjectModel;

namespace TrimesterPlaner.Extensions
{
    public static class CollectionExtensions
    {
        public static void InsertSorted<T>(this ObservableCollection<T> collection, T item, Comparison<T> comparison)
        {
            bool last = true;
            for (int i = 0; i < collection.Count; i++)
            {
                if (comparison.Invoke(collection[i], item) >= 1)
                {
                    collection.Insert(i, item);
                    last = false;
                    break;
                }
            }
            if (last)
            {
                collection.Add(item);
            }
        }

        public static void ClearAndAdd<T>(this ObservableCollection<T> collection, IEnumerable<T> items, Comparison<T>? comparison = null)
        {
            collection.Clear();
            foreach (T item in items)
            {
                if (comparison is null)
                {
                    collection.Add(item);
                }
                else
                {
                    collection.InsertSorted(item, comparison);
                }
            }
        }
    }
}