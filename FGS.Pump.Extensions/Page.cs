using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FGS.Pump.Extensions
{
    public class Page<T> : IReadOnlyList<T>
    {
        public Page(IEnumerable<T> items, PaginationSpecification paginationSpecification, bool hasNextPage)
        {
            Items = items.ToList().AsReadOnly();
            PaginationSpecification = paginationSpecification;
            HasNextPage = hasNextPage;
        }

        public IReadOnlyList<T> Items { get; }

        public PaginationSpecification PaginationSpecification { get; }

        public bool HasNextPage { get; }

        public bool IsFull => Count == PaginationSpecification.PageSize;

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => Items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => Items.GetEnumerator();

        public int Count => Items.Count;

        public T this[int index] => Items[index];
    }
}
