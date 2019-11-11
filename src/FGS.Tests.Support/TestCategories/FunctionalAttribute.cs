using System;

using NUnit.Framework;

namespace FGS.Tests.Support.TestCategories
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class FunctionalAttribute : CategoryAttribute
    {
        /// <summary>
        /// Checks a particular feature for correctness by comparing the results for a given
        /// input against the specification.
        /// </summary>
        public FunctionalAttribute()
            : base("Functional")
        {
        }
    }
}
