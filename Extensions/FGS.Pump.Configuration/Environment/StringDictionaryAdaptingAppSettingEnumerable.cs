using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FGS.Pump.Configuration.Environment
{
    public class StringDictionaryAdaptingAppSettingEnumerable : IEnumerable<KeyValuePair<string, string>>
    {
        private readonly IDictionary<string, string> _adapted;
        private readonly IAppSettingsKeyAdaptationStrategy _adaptationStrategy;

        public StringDictionaryAdaptingAppSettingEnumerable(IDictionary<string, string> adapted, IAppSettingsKeyAdaptationStrategy adaptationStrategy)
        {
            _adapted = adapted;
            _adaptationStrategy = adaptationStrategy;
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator() =>
            _adapted.Where(kvp => _adaptationStrategy.IsAppSettingUnderlyingKey(kvp.Key)).Select(kvp => new KeyValuePair<string, string>(_adaptationStrategy.ToAppSettingName(kvp.Key), kvp.Value)).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
