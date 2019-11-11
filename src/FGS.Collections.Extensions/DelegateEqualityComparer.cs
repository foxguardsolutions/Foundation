using System;
using System.Collections.Generic;

namespace FGS.Collections.Extensions
{
    /// <remarks>Taken and modified from: http://stackoverflow.com/questions/4607485/linq-distinct-use-delegate-for-equality-comparer. </remarks>
    internal class DelegateEqualityComparer<T, TIdentity> : IEqualityComparer<T>
    {
        private readonly Func<T, TIdentity> identitySelector;

        internal DelegateEqualityComparer(Func<T, TIdentity> identitySelector)
        {
            this.identitySelector = identitySelector;
        }

        public bool Equals(T x, T y)
        {
            return Equals(identitySelector(x), identitySelector(y));
        }

        public int GetHashCode(T obj)
        {
            return identitySelector(obj).GetHashCode();
        }
    }
}
