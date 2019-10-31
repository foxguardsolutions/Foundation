using System;
using System.Collections.Generic;
using System.Linq;

namespace FGS.Collections.Extensions
{
    /// <summary>
    /// Extends <see cref="IEnumerable{T}"/> with functionality that supplements the frameworks in-built collection querying mechanisms.
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>Returns distinct elements from a sequence by using <paramref name="identitySelector"/> to compare values.</summary>
        /// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains distinct elements from the source sequence.</returns>
        /// <param name="source">The sequence to remove duplicate elements from.</param>
        /// <param name="identitySelector">A projection that produces values that are used for determining uniqueness.</param>
        /// <typeparam name="T">The type of the elements of <paramref name="source" />.</typeparam>
        /// <remarks>Taken and modified from: http://stackoverflow.com/questions/4607485/linq-distinct-use-delegate-for-equality-comparer. </remarks>
        public static IEnumerable<T> DistinctBy<T, TIdentity>(this IEnumerable<T> source, Func<T, TIdentity> identitySelector)
        {
            return source.Distinct(By(identitySelector));
        }

        /// <remarks>Taken and modified from: http://stackoverflow.com/questions/4607485/linq-distinct-use-delegate-for-equality-comparer. </remarks>
        private static IEqualityComparer<TSource> By<TSource, TIdentity>(Func<TSource, TIdentity> identitySelector)
        {
            return new DelegateEqualityComparer<TSource, TIdentity>(identitySelector);
        }
    }
}
