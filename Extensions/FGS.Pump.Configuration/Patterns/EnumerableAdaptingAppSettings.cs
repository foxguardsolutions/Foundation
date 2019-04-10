using System.Collections;
using System.Collections.Generic;
using System.Linq;

using FGS.Pump.Configuration.Abstractions;

namespace FGS.Pump.Configuration.Patterns
{
    public class EnumerableAdaptingAppSettings : IAppSettings
    {
        private readonly IEnumerable<KeyValuePair<string, string>> _adapted;
        private readonly IEqualityComparer<string> _appSettingNameComparer;

        public EnumerableAdaptingAppSettings(IEnumerable<KeyValuePair<string, string>> adapted, IEqualityComparer<string> appSettingNameComparer)
        {
            _adapted = adapted;
            _appSettingNameComparer = appSettingNameComparer;
        }

        public string this[string key] => _adapted.Where(kvp => _appSettingNameComparer.Equals(kvp.Key, key)).Select(kvp => kvp.Value).FirstOrDefault();

        public IEnumerable<string> Keys => _adapted.Select(kvp => kvp.Key);

        public IEnumerable<string> Values => _adapted.Select(kvp => kvp.Value);

        public int Count => _adapted.Count();

        public bool ContainsKey(string key) => _adapted.Where(kvp => _appSettingNameComparer.Equals(kvp.Key, key)).Any();

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => _adapted.GetEnumerator();

        public bool TryGetValue(string key, out string value)
        {
            value = this[key];
            return value != null;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
