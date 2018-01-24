using System.Collections.Generic;
using System.Linq;

namespace FGS.Pump.Extensions
{
    public static class CollectionExtensions
    {
        public static ICollection<T> TakeLast<T>(this ICollection<T> collection, int count)
        {
            return collection.Reverse().Take(count).Reverse().ToList();
        }

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