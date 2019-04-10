using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FGS.Pump.Configuration.Abstractions
{
    public class ConfigurationManagerAdaptingAppSettings : IAppSettings
    {
        private System.Configuration.Abstractions.IAppSettings _appSettings;
        private IEqualityComparer<string> _appSettingNameComparer;

        public ConfigurationManagerAdaptingAppSettings(System.Configuration.Abstractions.IConfigurationManager configurationManager, IEqualityComparer<string> appSettingNameComparer)
        {
            _appSettings = configurationManager.AppSettings;
            _appSettingNameComparer = appSettingNameComparer;
        }

        public string this[string key] =>
            _appSettings.AllKeys.Where(k => _appSettingNameComparer.Equals(k, key)).Select(k => _appSettings[k]).FirstOrDefault();

        public IEnumerable<string> Keys => _appSettings.AllKeys;

        public IEnumerable<string> Values => Keys.Select(k => _appSettings[k]);

        public int Count => _appSettings.Count;

        public bool ContainsKey(string key) => _appSettings.AllKeys.Any(k => _appSettingNameComparer.Equals(k, key));

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator() =>
            _appSettings.AllKeys.Select(k => new KeyValuePair<string, string>(k, _appSettings[k])).GetEnumerator();

        public bool TryGetValue(string key, out string value)
        {
            value = this[key];
            return value != null;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
