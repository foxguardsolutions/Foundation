using System.Collections.Generic;
#if EF6
using System.Data.Entity;
#endif
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
#if EFCORE
using Microsoft.EntityFrameworkCore;
#endif

namespace FGS.Linq.Extensions.EntityFramework
{
    /// <summary>
    /// Extends <see cref="IQueryable{T}"/> with a facade of materialization methods.
    /// </summary>
    public static class MaterializingQueryableExtensions
    {
        /// <summary>
        /// Asynchronously executes the query represented by <paramref name="source"/> and returns all of the results.
        /// </summary>
        /// <typeparam name="T">The type of items that <paramref name="source"/> queries for.</typeparam>
        /// <param name="source">The source query.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the materialization to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="IReadOnlyList{T}"/> that contains elements from the input query.</returns>
        public static async Task<IReadOnlyList<T>> MaterializeAsync<T>(this IQueryable<T> source, CancellationToken cancellationToken = default) =>
            (await source.ToListAsync(cancellationToken).ConfigureAwait(false)).AsReadOnly();

        /// <summary>
        /// Asynchronously returns a single element from the query represented by <paramref name="source"/>, and throws an exception if there is not exactly one element.
        /// </summary>
        /// <typeparam name="T">The type of items that <paramref name="source"/> queries for.</typeparam>
        /// <param name="source">The source query.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the materialization to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the single element from the input query.</returns>
        public static Task<T> MaterializeSingleAsync<T>(this IQueryable<T> source, CancellationToken cancellationToken = default) =>
            source.SingleAsync(cancellationToken);

        /// <summary>
        /// Asynchronously returns a single element from the query represented by <paramref name="source"/>, returns <see langword="null"/> if there are no elements, and throws an exception if there is more than one element.
        /// </summary>
        /// <typeparam name="T">The type of items that <paramref name="source"/> queries for.</typeparam>
        /// <param name="source">The source query.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the materialization to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the single element from the input query.</returns>
        public static Task<T> MaterializeSingleOrDefaultAsync<T>(this IQueryable<T> source, CancellationToken cancellationToken = default) =>
            source.SingleOrDefaultAsync(cancellationToken);

        /// <summary>
        /// Asynchronously returns the first element from the query represented by <paramref name="source"/>, and throws an exception if there are no elements.
        /// </summary>
        /// <typeparam name="T">The type of items that <paramref name="source"/> queries for.</typeparam>
        /// <param name="source">The source query.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the materialization to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the first element from the input query.</returns>
        public static Task<T> MaterializeFirstAsync<T>(this IQueryable<T> source, CancellationToken cancellationToken = default) =>
            source.FirstAsync(cancellationToken);

        /// <summary>
        /// Asynchronously returns the first element from the query represented by <paramref name="source"/>, and returns <see langword="null"/> if there are no elements.
        /// </summary>
        /// <typeparam name="T">The type of items that <paramref name="source"/> queries for.</typeparam>
        /// <param name="source">The source query.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the materialization to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the first element from the input query.</returns>
        public static Task<T> MaterializeFirstOrDefaultAsync<T>(this IQueryable<T> source, CancellationToken cancellationToken = default) =>
            source.FirstOrDefaultAsync(cancellationToken);
#if EF_CORE

        /// <summary>
        /// Asynchronously returns the last element from the query represented by <paramref name="source"/>, and throws an exception if there are no elements.
        /// </summary>
        /// <typeparam name="T">The type of items that <paramref name="source"/> queries for.</typeparam>
        /// <param name="source">The source query.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the materialization to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the last element from the input query.</returns>
        public static Task<T> MaterializeLastAsync<T>(this IQueryable<T> source, CancellationToken cancellationToken = default) =>
            source.LastAsync(cancellationToken);

        /// <summary>
        /// Asynchronously returns the last element from the query represented by <paramref name="source"/>, and returns <see langword="null"/> if there are no elements.
        /// </summary>
        /// <typeparam name="T">The type of items that <paramref name="source"/> queries for.</typeparam>
        /// <param name="source">The source query.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the materialization to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the last element from the input query.</returns>
        public static Task<T> MaterializeLastOrDefaultAsync<T>(this IQueryable<T> source, CancellationToken cancellationToken = default) =>
            source.LastOrDefaultAsync(cancellationToken);
#endif
    }
}
