using FGS.Collections.Extensions.Pagination.Abstractions;
using FGS.Linq.Expressions;
using FGS.Linq.Extensions.Pagination.Abstractions;
using System.Linq;

namespace FGS.Linq.Extensions.Pagination
{
    /// <summary>
    /// Extends <see cref="IQueryable{T}"/> with pagination capabilities.
    /// </summary>
    public static class PaginationQueryableExtensions
    {
        /// <summary>
        /// Returns a query of a paginated subsection of the original <paramref name="source"/>, paginated based on <paramref name="paginationSpecification"/>.
        /// </summary>
        /// <param name="source">The query of items to paginate.</param>
        /// <param name="paginationSpecification">Specifies how the <paramref name="source"/> should be paginated.</param>
        /// <typeparam name="T">The type of items to paginate.</typeparam>
        /// <returns>A <see cref="PageQuery{T}"/> representing the paginated results.</returns>
        /// <remarks>Assumes <paramref name="source.Provider"/> is able to translate and evaluate queries generated via <see cref="FGS.Linq.Expressions.QueryProviderExtensions.CreateScalarQuery{TResult}(IQueryProvider, System.Linq.Expressions.Expression{System.Func{TResult}})"/>.</remarks>
        public static PageQuery<T> Paginate<T>(this IQueryable<T> source, PaginationSpecification paginationSpecification)
        {
            var queryOfItemsOnResultPagePlusEverAfter = source.Skip(paginationSpecification.PageNumber * paginationSpecification.PageSize);
            var queryOfItemsOnResultPage = queryOfItemsOnResultPagePlusEverAfter.Take(paginationSpecification.PageSize);
            var queryOfItemAfterResultPage = queryOfItemsOnResultPagePlusEverAfter.Skip(paginationSpecification.PageSize).Take(1);

            var queryOfAnyItemAfterResultPage = source.Provider.CreateScalarQuery(() => queryOfItemAfterResultPage.Any());

            return new PageQuery<T>(queryOfItemsOnResultPage, paginationSpecification, queryOfAnyItemAfterResultPage);
        }
    }
}
