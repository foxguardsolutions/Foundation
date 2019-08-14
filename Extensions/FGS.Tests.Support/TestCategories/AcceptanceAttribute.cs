using System;

using NUnit.Framework;

namespace FGS.Tests.Support.TestCategories
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AcceptanceAttribute : CategoryAttribute
    {
        /// <summary>
        /// Tests the full system to see whether the application's functionality satisfies the
        /// specification.
        /// </summary>
        public AcceptanceAttribute()
            : base("Acceptance")
        {
        }
    }
}
