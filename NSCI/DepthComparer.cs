using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NSCI
{
    public partial class OrderedList<T> : Collection<T>, IList<T>
    {
        private readonly IComparer<T> comparer;

        public OrderedList(IComparer<T> comparer)
        {
            this.comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
        }

        protected override sealed void InsertItem(int index, T item)
        {
            int insertIndex = GetInsertIndex(item);
            base.InsertItem(insertIndex, item);
        }

        protected override sealed void SetItem(int index, T item)
        {
            RemoveItem(index);
            int insertIndex = GetInsertIndex(item);
            base.InsertItem(insertIndex, item);
        }

        private int GetInsertIndex(T item)
        {
            if (Count == 0)
                return 0;
            var index = this.BinarySearch(item, this.comparer);
            if (index < 0)
                index = ~index;
            return index;
        }



    }

}