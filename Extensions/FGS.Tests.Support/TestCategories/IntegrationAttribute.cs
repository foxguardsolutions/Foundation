using System;

using NUnit.Framework;

namespace FGS.Tests.Support.TestCategories
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class IntegrationAttribute : CategoryAttribute
    {
        /// <summary>
        /// Combines units of code and tests that the resulting combination functions correctly.
        /// </summary>
        public IntegrationAttribute()
            : base("Integration")
        {
        }
    }
}
