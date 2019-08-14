using System;

using NUnit.Framework;

namespace FGS.Tests.Support.TestCategories
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class StaticAnalysisAttribute : CategoryAttribute
    {
        /// <summary>
        /// Tests code for a given quality by analyzing its structure instead of executing it.
        /// </summary>
        public StaticAnalysisAttribute()
            : base("StaticAnalysis")
        {
        }
    }
}
