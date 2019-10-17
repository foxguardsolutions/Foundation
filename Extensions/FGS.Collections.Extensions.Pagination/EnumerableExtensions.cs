using System.Collections.Generic;
using System.Linq;

using FGS.Collections.Extensions.Pagination.Abstractions;

namespace FGS.Collections.Extensions.Pagination
{
    public static class EnumerableExtensions
    {
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
