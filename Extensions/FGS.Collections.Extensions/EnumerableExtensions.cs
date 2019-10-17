using System;
using System.Collections.Generic;
using System.Linq;

namespace FGS.Collections.Extensions
{
    public static class EnumerableExtensions
    {
        /// <remarks>Stolen wholesale from http://stackoverflow.com/questions/4607485/linq-distinct-use-delegate-for-equality-comparer. </remarks>
        public static IEnumerable<T> DistinctBy<T, TIdentity>(this IEnumerable<T> source, Func<T, TIdentity> identitySelector)
        {
            return source.Distinct(By(identitySelector));
        }

        /// <remarks>Stolen wholesale from http://stackoverflow.com/questions/4607485/linq-distinct-use-delegate-for-equality-comparer. </remarks>
        private static IEqualityComparer<TSource> By<TSource, TIdentity>(Func<TSource, TIdentity> identitySelector)
        {
            return new DelegateEqualityComparer<TSource, TIdentity>(identitySelector);
        }
    }
}
