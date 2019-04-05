using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace FGS.Pump.Configuration.Patterns
{
    public class EnumerableConnectionStringSettingsMultiplexReducer : IEnumerable<ConnectionStringSettings>
    {
        private readonly IEnumerable<IEnumerable<ConnectionStringSettings>> _adapted;
        private readonly IEqualityComparer<string> _connectionStringNameComparer;

        public EnumerableConnectionStringSettingsMultiplexReducer(IEnumerable<IEnumerable<ConnectionStringSettings>> adapted, IEqualityComparer<string> connectionStringNameComparer)
        {
            _adapted = adapted;
            _connectionStringNameComparer = connectionStringNameComparer;
        }

        public IEnumerator<ConnectionStringSettings> GetEnumerator() =>
            _adapted
                .Aggregate(Enumerable.Empty<ConnectionStringSettings>(), (p, c) => p.Concat(c), css => FirstDistinctBy(css, cssi => cssi.Name, _connectionStringNameComparer))
                .GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private static IEnumerable<T> FirstDistinctBy<T, TKey>(IEnumerable<T> items, Func<T, TKey> keySelector, IEqualityComparer<TKey> keyComparer) =>
            items
                .Select((item, ordinal) => new { item, ordinal })
                .GroupBy(itemAndOrdinal => keySelector(itemAndOrdinal.item), (key, subitems) => subitems.OrderBy(itemAndOrdinal => itemAndOrdinal.ordinal), keyComparer)
                .Select(grouping => grouping.First().item);
    }
}
