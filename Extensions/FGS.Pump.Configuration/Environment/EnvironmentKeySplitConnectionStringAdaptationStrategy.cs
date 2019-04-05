using System.Text.RegularExpressions;

namespace FGS.Pump.Configuration.Environment
{
    public class EnvironmentKeySplitConnectionStringAdaptationStrategy : ISplitConnectionStringAdaptationStrategy
    {
        public const string Separator = "_";
        public const string Prefix = "PUMP";
        public const string ValueSuffix = "ConnectionString";
        public const string ProviderSuffix = "Provider";

        private const RegexOptions RegexOptions = System.Text.RegularExpressions.RegexOptions.Compiled | System.Text.RegularExpressions.RegexOptions.IgnoreCase;
        private readonly Regex _valueKeyMatcher;
        private readonly Regex _providerKeyMatcher;

        public EnvironmentKeySplitConnectionStringAdaptationStrategy()
            : this(Separator, Prefix, ValueSuffix, ProviderSuffix)
        {
        }

        private EnvironmentKeySplitConnectionStringAdaptationStrategy(string separator, string prefix, string valueSuffix, string providerSuffix)
        {
            var escapedSeparator = Regex.Escape(separator);
            var escapedPrefix = string.IsNullOrWhiteSpace(prefix) ? string.Empty : (Regex.Escape(prefix) + escapedSeparator);
            var escapedValueSuffix = string.IsNullOrWhiteSpace(valueSuffix) ? string.Empty : (escapedSeparator + Regex.Escape(valueSuffix));
            var escapedProviderSuffix = string.IsNullOrWhiteSpace(providerSuffix) ? string.Empty : (escapedSeparator + Regex.Escape(providerSuffix));

            _valueKeyMatcher = new Regex("^" + escapedPrefix + "(.*)" + escapedValueSuffix + "$", RegexOptions);
            _providerKeyMatcher = new Regex("^" + escapedPrefix + "(.*)" + escapedProviderSuffix + "$", RegexOptions);
        }

        public bool IsConnectionStringValueUnderlyingKey(string key) => _valueKeyMatcher.IsMatch(key);

        public bool IsConnectionStringProviderUnderlyingKey(string key) => _providerKeyMatcher.IsMatch(key);

        public string ConvertToConnectionStringNameFromValueUnderlyingKey(string key) => _valueKeyMatcher.Match(key).Groups[1].Value;

        public string ConvertToConnectionStringNameFromProviderUnderlyingKey(string key) => _providerKeyMatcher.Match(key).Groups[1].Value;
    }
}