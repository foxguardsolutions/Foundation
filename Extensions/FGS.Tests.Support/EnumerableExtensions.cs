using System.Collections.Generic;

namespace FGS.Tests.Support.Extensions
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Amplifies this instance into an <see cref="IEnumerable{T}"/>
        /// consisting of a single item.
        /// </summary>
        /// <typeparam name="T"> Type of the object. </typeparam>
        /// <param name="item"> The instance that will be amplified.</param>
        /// <remarks>
        /// <para> If <paramref name="Item"/> is null, the resulting <see cref="IEnumerable{T}"/>
        /// will be empty.</para>
        /// </remarks>
        /// <returns> An IEnumerable&lt;T&gt; consisting of a single item. </returns>
        public static IEnumerable<T> Yield<T>(this T item)
        {
            if (item == null) yield break;

            yield return item;
        }
    }
}
