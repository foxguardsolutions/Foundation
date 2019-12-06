using System.Linq;
using System.Threading;
using System.Threading.Tasks;
#if EF6
using System.Data.Entity;
#endif
using FGS.Collections.Extensions.OneOf.Units;
using FGS.Linq.Extensions.EntityFramework;
#if EFCORE
using Microsoft.EntityFrameworkCore;
#endif
using OneOf;

namespace FGS.Linq.Extensions.OneOf.EntityFramework
{
    /// <summary>
    /// Extends <see cref="IQueryable{T}"/> with a materialization methods that describe their failure conditions using discriminated unions.
    /// </summary>
    public static class OneOfQueryableExtensions
    {
        /// <summary>
        /// Asynchronously returns a single element from the query represented by <paramref name="source"/>, returns <see cref="NoElements"/> if there are no elements, returns <see cref="MoreThanOneElement"/> if there is more than one element.
        /// </summary>
        /// <typeparam name="T">The type of items that <paramref name="source"/> queries for.</typeparam>
        /// <param name="source">The source query.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the materialization to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the single element from the input query, or the reason why it could be returned.</returns>
        public static async Task<OneOf<T, NoElements, MoreThanOneElement>> MaterializeSingleOrReasonWhyNot<T>(this IQueryable<T> source, CancellationToken cancellationToken = default)
        {
            var firstOrDefault = await source.MaterializeFirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
            var anyMore = await source.Skip(1).AnyAsync(cancellationToken).ConfigureAwait(false);

            if (anyMore)
                return default(MoreThanOneElement);
            else if (firstOrDefault != null)
                return firstOrDefault;
            else
                return default(NoElements);
        }

        /// <summary>
        /// Asynchronously returns the first element from the query represented by <paramref name="source"/>, and returns <see cref="NoElements"/> if there are no elements.
        /// </summary>
        /// <typeparam name="T">The type of items that <paramref name="source"/> queries for.</typeparam>
        /// <param name="source">The source query.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the materialization to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the first element from the input query, or the reason why it could be returned.</returns>
        public static async Task<OneOf<T, NoElements>> MaterializeFirstOrReasonWhyNot<T>(this IQueryable<T> source, CancellationToken cancellationToken = default)
        {
            var firstOrDefault = await source.MaterializeFirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);

            if (firstOrDefault != null)
                return firstOrDefault;
            else
                return default(NoElements);
        }
#if EF_CORE

        /// <summary>
        /// Asynchronously returns the last element from the query represented by <paramref name="source"/>, and returns <see cref="NoElements"/> if there are no elements.
        /// </summary>
        /// <typeparam name="T">The type of items that <paramref name="source"/> queries for.</typeparam>
        /// <param name="source">The source query.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the materialization to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the last element from the input query, or the reason why it could be returned.</returns>
        public static Task<OneOf<T, NoElements>> MaterializeLastOrReasonWhyNot<T>(this IQueryable<T> source, CancellationToken cancellationToken = default)
        {
            var lastOrDefault = await source.MaterializeLastOrDefaultAsync(cancellationToken).ConfigureAwait(false);

            if (lastOrDefault != null)
                return lastOrDefault;
            else
                return default(NoElements);
        }
#endif
    }
}
