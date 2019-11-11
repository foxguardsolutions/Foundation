using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FGS.Collections.Extensions.Pagination.Abstractions
{
    /// <summary>
    /// Represents a page of items taken from a larger set of items.
    /// </summary>
    /// <typeparam name="T">The type of items in the page.</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1710:Identifiers should have correct suffix", Justification = "Name is a noun known to be a collection of items")]
    public sealed class Page<T> : IReadOnlyList<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Page{T}"/> class.
        /// </summary>
        /// <param name="items">The items on this page.</param>
        /// <param name="paginationSpecification">The specification of how the items for this page were selected.</param>
        /// <param name="hasNextPage">Whether or not future pages are expected, given the original source and an incremented <see cref="PaginationSpecification"/>.</param>
        public Page(IEnumerable<T> items, PaginationSpecification paginationSpecification, bool hasNextPage)
        {
            Items = items.ToList().AsReadOnly();
            PaginationSpecification = paginationSpecification;
            HasNextPage = hasNextPage;
        }

        /// <summary>
        /// Gets the items on this page.
        /// </summary>
        public IReadOnlyList<T> Items { get; }

        /// <summary>
        /// Gets the specification of how the items for this page were selected.
        /// </summary>
        public PaginationSpecification PaginationSpecification { get; }

        /// <summary>
        /// Gets a value indicating whether or not future pages are expected, given the original source and an incremented <see cref="PaginationSpecification"/>.
        /// </summary>
        public bool HasNextPage { get; }

        /// <summary>
        /// Gets a value indicating whether or not this page has the maximum number of items per page allowed by the <see cref="PaginationSpecification"/>.
        /// </summary>
        public bool IsFull => Count == PaginationSpecification.PageSize;

        /// <inheritdoc cref="IEnumerable{T}.GetEnumerator()"/>
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => Items.GetEnumerator();

        /// <inheritdoc cref="IEnumerable.GetEnumerator()"/>
        IEnumerator IEnumerable.GetEnumerator() => Items.GetEnumerator();

        /// <inheritdoc cref="IReadOnlyCollection{T}.Count"/>
        public int Count => Items.Count;

        /// <inheritdoc cref="IReadOnlyList{T}.this"/>
        public T this[int index] => Items[index];
    }
}
