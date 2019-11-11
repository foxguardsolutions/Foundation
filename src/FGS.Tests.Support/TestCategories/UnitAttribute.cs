using System;

using NUnit.Framework;

namespace FGS.Tests.Support.TestCategories
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class UnitAttribute : CategoryAttribute
    {
        /// <summary>
        /// Tests the smallest unit of functionality, typically a method/function.
        /// </summary>
        public UnitAttribute()
            : base("Unit")
        {
        }
    }
}
