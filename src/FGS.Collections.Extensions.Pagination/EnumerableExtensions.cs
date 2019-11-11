using System.Collections.Generic;
using System.Linq;

using FGS.Collections.Extensions.Pagination.Abstractions;

namespace FGS.Collections.Extensions.Pagination
{
    /// <summary>
    /// Extends <see cref="IEnumerable{T}"/> with functionality to facilitate pagination.
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Returns a paginated subsection of the original <paramref name="items"/>, paginated based on <paramref name="paginationSpecification"/>.
        /// </summary>
        /// <param name="items">The items to paginate.</param>
        /// <param name="paginationSpecification">Specifies how the <paramref name="items"/> should be paginated.</param>
        /// <typeparam name="T">The type of items to paginate.</typeparam>
        /// <returns>A page of items of type <typeparamref name="T"/>.</returns>
        public static Page<T> Paginate<T>(this IEnumerable<T> items, PaginationSpecification paginationSpecification)
        {
            var queryOfItemsOnResultPagePlusEverAfter = items.Skip(paginationSpecification.PageNumber * paginationSpecification.PageSize);
            var queryOfItemsOnResultPage = queryOfItemsOnResultPagePlusEverAfter.Take(paginationSpecification.PageSize);
            var queryOfItemAfterResultPage = queryOfItemsOnResultPagePlusEverAfter.Skip(paginationSpecification.PageSize).Take(1);

            var itemsOnResultPage = queryOfItemsOnResultPage.ToArray();
            var hasNextPage = queryOfItemAfterResultPage.Any();

            return new Page<T>(itemsOnResultPage, paginationSpecification, hasNextPage);
        }
    }
}
