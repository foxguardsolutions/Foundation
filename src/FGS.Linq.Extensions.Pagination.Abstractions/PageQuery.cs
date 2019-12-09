using FGS.Collections.Extensions.Pagination.Abstractions;
using System.Linq;

namespace FGS.Linq.Extensions.Pagination.Abstractions
{
    /// <summary>
    /// Represents a query of a page of items taken from a larger set of items.
    /// </summary>
    /// <typeparam name="T">The type of items in the page.</typeparam>
    public class PageQuery<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PageQuery{T}"/> class.
        /// </summary>
        /// <param name="items">The query of the items on this page.</param>
        /// <param name="paginationSpecification">The specification of how the items for this page are selected.</param>
        /// <param name="hasNextPage">A query of single <see cref="bool"/> evaluating whether or not future pages are expected, given the original source and an incremented <see cref="PaginationSpecification"/>.</param>
        public PageQuery(IQueryable<T> items, PaginationSpecification paginationSpecification, IQueryable<bool> hasNextPage)
        {
            Items = items;
            PaginationSpecification = paginationSpecification;
            HasNextPage = hasNextPage;
        }

        /// <summary>
        /// Gets a query of the items on this page.
        /// </summary>
        public IQueryable<T> Items { get; }

        /// <summary>
        /// Gets the specification of how the items for this page are selected.
        /// </summary>
        public PaginationSpecification PaginationSpecification { get; }

        /// <summary>
        /// Gets a query that evaluates a single <see cref="bool"/> value that indicates whether or not future pages are expected, given the original source and an incremented <see cref="PaginationSpecification"/>.
        /// </summary>
        public IQueryable<bool> HasNextPage { get; }
    }
}
