using FGS.Collections.Extensions.Pagination.Abstractions;
using FGS.Linq.Extensions.EntityFramework;
using FGS.Linq.Extensions.Pagination.Abstractions;
using System.Threading.Tasks;

namespace FGS.Linq.Extensions.Pagination.EntityFramework
{
    /// <summary>
    /// Extends <see cref="PageQuery{T}"/> with materialization functionality.
    /// </summary>
    public static class PageQueryExtensions
    {
        /// <summary>
        /// Asynchronously materializes <paramref name="self"/> into a <see cref="Page{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of item on the page.</typeparam>
        /// <param name="self">The source representation of a paginated query that will be materialized.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Page{T}"/> that contains elements from the input query.</returns>
        public static async Task<Page<T>> MaterializeAsync<T>(this PageQuery<T> self)
        {
            var items = await self.Items.MaterializeAsync().ConfigureAwait(false);
            var hasNextPage = await self.HasNextPage.MaterializeSingleOrDefaultAsync().ConfigureAwait(false);

            return new Page<T>(items, self.PaginationSpecification, hasNextPage);
        }
    }
}
