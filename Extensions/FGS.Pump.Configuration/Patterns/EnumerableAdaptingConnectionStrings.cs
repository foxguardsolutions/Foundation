using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

using FGS.Pump.Configuration.Abstractions;

namespace FGS.Pump.Configuration.Patterns
{
    public class EnumerableAdaptingConnectionStrings : IConnectionStrings
    {
        private readonly IEqualityComparer<string> _connectionStringNameComparer;

        public EnumerableAdaptingConnectionStrings(IEnumerable<ConnectionStringSettings> adapted, IEqualityComparer<string> connectionStringNameComparer)
        {
            Values = adapted;
            _connectionStringNameComparer = connectionStringNameComparer;
        }

        public ConnectionStringSettings this[string key] => Values.Where(css => _connectionStringNameComparer.Equals(css.Name, key)).FirstOrDefault();

        public IEnumerable<string> Keys => Values.Select(css => css.Name);

        public IEnumerable<ConnectionStringSettings> Values { get; }

        public int Count => Values.Count();

        public bool ContainsKey(string key) => Values.Where(css => _connectionStringNameComparer.Equals(css.Name, key)).Any();

        public IEnumerator<KeyValuePair<string, ConnectionStringSettings>> GetEnumerator() =>
            Values.Select(css => new KeyValuePair<string, ConnectionStringSettings>(css.Name, css)).GetEnumerator();

        public bool TryGetValue(string key, out ConnectionStringSettings value)
        {
            value = this[key];
            return value != null;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
