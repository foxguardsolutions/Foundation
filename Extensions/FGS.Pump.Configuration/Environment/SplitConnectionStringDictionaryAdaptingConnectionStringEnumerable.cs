using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;

namespace FGS.Pump.Configuration.Environment
{
    public class SplitConnectionStringDictionaryAdaptingConnectionStringEnumerable : IEnumerable<ConnectionStringSettings>
    {
        private readonly IDictionary<string, string> _adapted;
        private readonly ISplitConnectionStringAdaptationStrategy _adaptationStrategy;
        private readonly IEqualityComparer<string> _connectionStringNameComparer;

        public SplitConnectionStringDictionaryAdaptingConnectionStringEnumerable(IDictionary<string, string> adapted, ISplitConnectionStringAdaptationStrategy adaptationStrategy, IEqualityComparer<string> connectionStringNameComparer)
        {
            _adapted = adapted;
            _adaptationStrategy = adaptationStrategy;
            _connectionStringNameComparer = connectionStringNameComparer;
        }

        public IEnumerator<ConnectionStringSettings> GetEnumerator()
        {
            var keysForValuesByConnectionStringName = new Dictionary<string, string>(_connectionStringNameComparer);
            var keysForProvidersByConnectionStringName = new Dictionary<string, string>(_connectionStringNameComparer);

            foreach (var key in _adapted.Keys)
            {
                if (_adaptationStrategy.IsConnectionStringValueUnderlyingKey(key))
                {
                    keysForValuesByConnectionStringName.Add(_adaptationStrategy.ConvertToConnectionStringNameFromValueUnderlyingKey(key), key);
                }
                else if (_adaptationStrategy.IsConnectionStringProviderUnderlyingKey(key))
                {
                    keysForProvidersByConnectionStringName.Add(_adaptationStrategy.ConvertToConnectionStringNameFromProviderUnderlyingKey(key), key);
                }
            }

            foreach (var connectionStringName in keysForValuesByConnectionStringName.Keys)
            {
                var keyForValue = keysForValuesByConnectionStringName[connectionStringName];
                var value = _adapted[keyForValue];
                string provider = null;
                if (keysForProvidersByConnectionStringName.TryGetValue(connectionStringName, out string keyForProvider))
                {
                    provider = _adapted[keyForProvider];
                    yield return new ConnectionStringSettings(connectionStringName, value, provider);
                }
                else
                {
                    yield return new ConnectionStringSettings(connectionStringName, value);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
