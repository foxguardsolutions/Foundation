using System.Collections.Generic;
using System.Linq;

using KellermanSoftware.CompareNetObjects;

namespace FGS.Tests.Support.AspNetCore.Mvc.Routing
{
    /// <summary>
    /// An implementation of <see cref="IEqualityComparer{object}"/> that ignores type differences between members of comparands.
    /// </summary>
    public sealed class RouteValuesLooseEqualityComparer : IEqualityComparer<object>
    {
        public static readonly RouteValuesLooseEqualityComparer Instance = new RouteValuesLooseEqualityComparer();

        private readonly CompareLogic _compareLogic;

        private RouteValuesLooseEqualityComparer(CompareLogic logic)
        {
            _compareLogic = logic;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RouteValuesLooseEqualityComparer"/> class.
        /// </summary>
        public RouteValuesLooseEqualityComparer()
            : this(CreateDefaultCompareLogic())
        {
        }

        /// <inheritdoc/>
        public new bool Equals(object expected, object actual)
        {
            var comparisonResult = _compareLogic.Compare(expected, actual);
            return !comparisonResult.Differences.Any();
        }

        /// <inheritdoc/>
        int IEqualityComparer<object>.GetHashCode(object obj) => obj.GetHashCode();

        private static CompareLogic CreateDefaultCompareLogic()
        {
            var compareConfig = new ComparisonConfig()
            {
                IgnoreObjectTypes = true,
            };

            return new CompareLogic(compareConfig);
        }
    }
}
