using System;
using System.Collections.Generic;

namespace FGS.Pump.Extensions
{
    /// <remarks>Stolen wholesale from http://stackoverflow.com/questions/4607485/linq-distinct-use-delegate-for-equality-comparer </remarks>
    internal class DelegateEqualityComparer<T, TIdentity> : IEqualityComparer<T>
    {
        private readonly Func<T, TIdentity> identitySelector;

        public DelegateEqualityComparer(Func<T, TIdentity> identitySelector)
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