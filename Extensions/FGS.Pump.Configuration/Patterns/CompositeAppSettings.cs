using System.Collections;
using System.Collections.Generic;
using System.Linq;

using FGS.Pump.Configuration.Abstractions;

namespace FGS.Pump.Configuration.Patterns
{
    public class CompositeAppSettings : IAppSettings
    {
        private readonly IEnumerable<IAppSettings> _adapted;
        private readonly IEqualityComparer<string> _appSettingNameComparer;

        public CompositeAppSettings(IEnumerable<IAppSettings> adapted, IEqualityComparer<string> appSettingNameComparer)
        {
            _adapted = adapted;
            _appSettingNameComparer = appSettingNameComparer;
        }

        public string this[string key] => _adapted.Select(aps => aps[key]).Where(v => v != null).FirstOrDefault();

        public IEnumerable<string> Keys => _adapted.SelectMany(aps => aps.Keys).Distinct(_appSettingNameComparer);

        public IEnumerable<string> Values => Keys.Select(k => this[k]);

        public int Count => Keys.Count();

        public bool ContainsKey(string key) => Keys.Where(k => _appSettingNameComparer.Equals(k, key)).Any();

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator() =>
            Keys.Select(k => new KeyValuePair<string, string>(k, this[k])).GetEnumerator();

        public bool TryGetValue(string key, out string value)
        {
            value = this[key];
            return value != null;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
