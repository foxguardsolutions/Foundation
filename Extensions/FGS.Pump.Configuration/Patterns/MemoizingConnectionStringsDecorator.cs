using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;

using FGS.Pump.Configuration.Abstractions;

namespace FGS.Pump.Configuration.Patterns
{
    public class MemoizingConnectionStringsDecorator : IConnectionStrings
    {
        private readonly IConnectionStrings _decorated;

        private readonly ConcurrentDictionary<string, ConnectionStringSettings> _indexerCache;
        private readonly Lazy<string[]> _keysCache;
        private readonly Lazy<ConnectionStringSettings[]> _valuesCache;
        private readonly Lazy<int> _countCache;
        private readonly ConcurrentDictionary<string, bool> _containsKeyCache;
        private readonly Lazy<IEnumerable<KeyValuePair<string, ConnectionStringSettings>>> _enumerationCache;

        public MemoizingConnectionStringsDecorator(IConnectionStrings decorated, IEqualityComparer<string> connectionStringNameComparer)
        {
            _decorated = decorated;

            const LazyThreadSafetyMode lazyThreadSafetyMode = LazyThreadSafetyMode.PublicationOnly;
            _indexerCache = new ConcurrentDictionary<string, ConnectionStringSettings>(connectionStringNameComparer);
            _keysCache = new Lazy<string[]>(() => _decorated.Keys.ToArray(), lazyThreadSafetyMode);
            _valuesCache = new Lazy<ConnectionStringSettings[]>(() => _decorated.Values.ToArray(), lazyThreadSafetyMode);
            _countCache = new Lazy<int>(() => _decorated.Count, lazyThreadSafetyMode);
            _containsKeyCache = new ConcurrentDictionary<string, bool>(connectionStringNameComparer);
            _enumerationCache = new Lazy<IEnumerable<KeyValuePair<string, ConnectionStringSettings>>>(() => _decorated.ToArray());
        }

        public ConnectionStringSettings this[string key] => _indexerCache.GetOrAdd(key, k => _decorated[k]);

        public IEnumerable<string> Keys => _keysCache.Value;

        public IEnumerable<ConnectionStringSettings> Values => _valuesCache.Value;

        public int Count => _countCache.Value;

        public bool ContainsKey(string key) => _containsKeyCache.GetOrAdd(key, k => _decorated.ContainsKey(k));

        public IEnumerator<KeyValuePair<string, ConnectionStringSettings>> GetEnumerator() => _enumerationCache.Value.GetEnumerator();

        public bool TryGetValue(string key, out ConnectionStringSettings value)
        {
            value = this[key];
            return value != null;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
