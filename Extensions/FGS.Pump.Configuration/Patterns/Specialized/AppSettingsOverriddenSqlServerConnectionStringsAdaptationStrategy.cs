using System.Text.RegularExpressions;

using FGS.Pump.Configuration.Environment;

namespace FGS.Pump.Configuration.Patterns.Specialized
{
    public class AppSettingsOverriddenSqlServerConnectionStringsAdaptationStrategy : IAppSettingsOverriddenSqlServerConnectionStringsAdaptationStrategy
    {
        public const string Separator = EnvironmentKeySplitConnectionStringAdaptationStrategy.Separator;
        public const string Interstitial = EnvironmentKeySplitConnectionStringAdaptationStrategy.ValueSuffix;

        private const RegexOptions RegexOptions = System.Text.RegularExpressions.RegexOptions.Compiled | System.Text.RegularExpressions.RegexOptions.IgnoreCase;
        private readonly Regex _keyMatcher;

        public AppSettingsOverriddenSqlServerConnectionStringsAdaptationStrategy()
            : this(Separator, Interstitial)
        {
        }

        private AppSettingsOverriddenSqlServerConnectionStringsAdaptationStrategy(string separator, string interstitial)
        {
            var escapedSeparator = Regex.Escape(separator);
            var escapedInterstitial = escapedSeparator + Regex.Escape(interstitial) + escapedSeparator;

            _keyMatcher = new Regex("^(.*)" + escapedInterstitial + "(.*)$", RegexOptions);
        }

        public bool IsConnectionStringOverridingAppSettingKey(string appSettingKey) => _keyMatcher.IsMatch(appSettingKey);

        public string ConvertToConnectionStringNameFromAppSettingKey(string appSettingKey) => _keyMatcher.Match(appSettingKey).Groups[1].Value;

        public string ConvertToConnectionStringPartNameFromAppSettingKey(string appSettingKey) => _keyMatcher.Match(appSettingKey).Groups[2].Value;
    }
}