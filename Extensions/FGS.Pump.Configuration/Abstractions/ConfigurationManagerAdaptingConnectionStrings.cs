using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace FGS.Pump.Configuration.Abstractions
{
    public class ConfigurationManagerAdaptingConnectionStrings : IConnectionStrings
    {
        private System.Configuration.Abstractions.IConnectionStrings _connectionStrings;
        private IEqualityComparer<string> _connectionStringNameComparer;

        public ConfigurationManagerAdaptingConnectionStrings(System.Configuration.Abstractions.IConfigurationManager configurationManager, IEqualityComparer<string> connectionStringNameComparer)
        {
            _connectionStrings = configurationManager.ConnectionStrings;
            _connectionStringNameComparer = connectionStringNameComparer;
        }

        public ConnectionStringSettings this[string key] =>
            _connectionStrings.Where(css => _connectionStringNameComparer.Equals(css.Name, key)).FirstOrDefault();

        public IEnumerable<string> Keys => _connectionStrings.Select(css => css.Name);

        public IEnumerable<ConnectionStringSettings> Values => _connectionStrings;

        public int Count => _connectionStrings.Count();

        public bool ContainsKey(string key) => _connectionStrings.Any(css => _connectionStringNameComparer.Equals(css.Name, key));

        public IEnumerator<KeyValuePair<string, ConnectionStringSettings>> GetEnumerator() =>
            _connectionStrings.Select(css => new KeyValuePair<string, ConnectionStringSettings>(css.Name, css)).GetEnumerator();

        public bool TryGetValue(string key, out ConnectionStringSettings value)
        {
            value = this[key];
            return value != null;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
