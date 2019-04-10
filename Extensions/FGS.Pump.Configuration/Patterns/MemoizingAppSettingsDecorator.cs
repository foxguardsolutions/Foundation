using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using FGS.Pump.Configuration.Abstractions;

namespace FGS.Pump.Configuration.Patterns
{
    public class MemoizingAppSettingsDecorator : IAppSettings
    {
        private readonly IAppSettings _decorated;

        private readonly ConcurrentDictionary<string, string> _indexerCache;
        private readonly Lazy<string[]> _keysCache;
        private readonly Lazy<string[]> _valuesCache;
        private readonly Lazy<int> _countCache;
        private readonly ConcurrentDictionary<string, bool> _containsKeyCache;
        private readonly Lazy<IEnumerable<KeyValuePair<string, string>>> _enumerationCache;

        public MemoizingAppSettingsDecorator(IAppSettings decorated, IEqualityComparer<string> appSettingNameComparer)
        {
            _decorated = decorated;

            const LazyThreadSafetyMode lazyThreadSafetyMode = LazyThreadSafetyMode.PublicationOnly;
            _indexerCache = new ConcurrentDictionary<string, string>(appSettingNameComparer);
            _keysCache = new Lazy<string[]>(() => _decorated.Keys.ToArray(), lazyThreadSafetyMode);
            _valuesCache = new Lazy<string[]>(() => _decorated.Values.ToArray(), lazyThreadSafetyMode);
            _countCache = new Lazy<int>(() => _decorated.Count, lazyThreadSafetyMode);
            _containsKeyCache = new ConcurrentDictionary<string, bool>(appSettingNameComparer);
            _enumerationCache = new Lazy<IEnumerable<KeyValuePair<string, string>>>(() => _decorated.ToArray());
        }

        public string this[string key] => _indexerCache.GetOrAdd(key, k => _decorated[k]);

        public IEnumerable<string> Keys => _keysCache.Value;

        public IEnumerable<string> Values => _valuesCache.Value;

        public int Count => _countCache.Value;

        public bool ContainsKey(string key) => _containsKeyCache.GetOrAdd(key, k => _decorated.ContainsKey(k));

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => _enumerationCache.Value.GetEnumerator();

        public bool TryGetValue(string key, out string value)
        {
            value = this[key];
            return value != null;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
