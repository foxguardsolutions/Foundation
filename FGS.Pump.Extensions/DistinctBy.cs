using System;
using System.Collections.Generic;
using System.Linq;

namespace FGS.Pump.Extensions
{    
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> source)
        {
            return source ?? Enumerable.Empty<T>();
        }

        /// <remarks>Stolen wholesale from http://stackoverflow.com/questions/4607485/linq-distinct-use-delegate-for-equality-comparer </remarks>
        public static IEnumerable<T> DistinctBy<T, TIdentity>(this IEnumerable<T> source, Func<T, TIdentity> identitySelector)
        {
            return source.Distinct(By(identitySelector));
        }

        /// <remarks>Stolen wholesale from http://stackoverflow.com/questions/4607485/linq-distinct-use-delegate-for-equality-comparer </remarks>
        private static IEqualityComparer<TSource> By<TSource, TIdentity>(Func<TSource, TIdentity> identitySelector)
        {
            return new DelegateEqualityComparer<TSource, TIdentity>(identitySelector);
        }
    }
}
