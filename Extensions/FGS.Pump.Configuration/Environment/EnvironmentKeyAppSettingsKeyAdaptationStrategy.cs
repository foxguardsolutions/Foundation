using System.Text.RegularExpressions;

namespace FGS.Pump.Configuration.Environment
{
    public class EnvironmentKeyAppSettingsKeyAdaptationStrategy : IAppSettingsKeyAdaptationStrategy
    {
        public const string Separator = "_";
        public const string Prefix = "PUMP";

        private const RegexOptions RegexOptions = System.Text.RegularExpressions.RegexOptions.Compiled | System.Text.RegularExpressions.RegexOptions.IgnoreCase;
        private readonly Regex _appSettingMatcher;

        public EnvironmentKeyAppSettingsKeyAdaptationStrategy()
            : this(Separator, Prefix)
        {
        }

        private EnvironmentKeyAppSettingsKeyAdaptationStrategy(string separator, string prefix)
        {
            var escapedSeparator = Regex.Escape(separator);
            var escapedPrefix = string.IsNullOrWhiteSpace(prefix) ? string.Empty : (Regex.Escape(prefix) + escapedSeparator);

            _appSettingMatcher = new Regex("^" + escapedPrefix + "(.*)" + "$", RegexOptions);
        }

        public bool IsAppSettingUnderlyingKey(string underlyingKey) => _appSettingMatcher.IsMatch(underlyingKey);

        public string ToAppSettingName(string underlyingKey) => _appSettingMatcher.Match(underlyingKey).Groups[1].Value;
    }
}
