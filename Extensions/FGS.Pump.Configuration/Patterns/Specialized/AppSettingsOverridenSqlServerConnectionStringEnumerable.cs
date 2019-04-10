using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;

using FGS.Pump.Configuration.Abstractions;

namespace FGS.Pump.Configuration.Patterns.Specialized
{
    public class AppSettingsOverridenSqlServerConnectionStringEnumerable : IEnumerable<ConnectionStringSettings>
    {
        public const string ExpectedOrDefaultProvider = "System.Data.SqlClient";

        private readonly IEnumerable<ConnectionStringSettings> _adapted;
        private readonly IAppSettings _overrideSource;
        private readonly IAppSettingsOverriddenSqlServerConnectionStringsAdaptationStrategy _adaptationStrategy;
        private readonly IEqualityComparer<string> _connectionStringNameComparer;
        private readonly IEqualityComparer<string> _connectionStringPartNameComparer;
        private readonly IEqualityComparer<string> _connectionStringProviderComparer;
        private readonly ISqlServerConnectionStringBuilderApplicator _sqlServerConnectionStringBuilderApplicator;

        public AppSettingsOverridenSqlServerConnectionStringEnumerable(
            IEnumerable<ConnectionStringSettings> adapted,
            IAppSettings overrideSource,
            IAppSettingsOverriddenSqlServerConnectionStringsAdaptationStrategy adaptationStrategy,
            IEqualityComparer<string> connectionStringNameComparer,
            IEqualityComparer<string> connectionStringPartNameComparer,
            IEqualityComparer<string> connectionStringProviderComparer,
            ISqlServerConnectionStringBuilderApplicator sqlServerConnectionStringBuilderApplicator)
        {
            _adapted = adapted;
            _overrideSource = overrideSource;
            _adaptationStrategy = adaptationStrategy;
            _connectionStringNameComparer = connectionStringNameComparer;
            _connectionStringPartNameComparer = connectionStringPartNameComparer;
            _connectionStringProviderComparer = connectionStringProviderComparer;
            _sqlServerConnectionStringBuilderApplicator = sqlServerConnectionStringBuilderApplicator;
        }

        public IEnumerator<ConnectionStringSettings> GetEnumerator()
        {
            var overrideAppSettingKeysByConnectionStringPartNameByConnectionStringName = GetOverrideAppSettingKeysByConnectionStringPartNameByConnectionStringName();

            foreach (var originalConnectionStringSettings in _adapted)
            {
                var providerName = originalConnectionStringSettings.ProviderName;
                var overrideAppKeysByConnectionStringPartName = overrideAppSettingKeysByConnectionStringPartNameByConnectionStringName[originalConnectionStringSettings.Name].FirstOrDefault();
                var hasOverrides = overrideAppKeysByConnectionStringPartName != null && overrideAppKeysByConnectionStringPartName.Count > 0;
                if (IsSupportedProvider(providerName) && hasOverrides)
                {
                    yield return CreateUpdatedConnectionStringSettings(originalConnectionStringSettings, overrideAppKeysByConnectionStringPartName);
                }
                else
                {
                    yield return originalConnectionStringSettings;
                }
            }
        }

        private ConnectionStringSettings CreateUpdatedConnectionStringSettings(ConnectionStringSettings originalConnectionStringSettings, IDictionary<string, string> overrideAppKeysByConnectionStringPartName)
        {
            var newConnectionStringValueBuilder = new SqlConnectionStringBuilder();
            newConnectionStringValueBuilder.ConnectionString = originalConnectionStringSettings.ConnectionString;

            foreach (var connectionStringPartName in overrideAppKeysByConnectionStringPartName.Keys)
            {
                var value = _overrideSource[overrideAppKeysByConnectionStringPartName[connectionStringPartName]];
                _sqlServerConnectionStringBuilderApplicator.Apply(newConnectionStringValueBuilder, connectionStringPartName, value);
            }

            return CreateUpdatedConnectionStringSettings(originalConnectionStringSettings, newConnectionStringValueBuilder.ConnectionString);
        }

        private static ConnectionStringSettings CreateUpdatedConnectionStringSettings(ConnectionStringSettings originalConnectionStringSettings, string newConnectionStringValue)
        {
            return string.IsNullOrWhiteSpace(originalConnectionStringSettings.ProviderName)
                   ? new ConnectionStringSettings(originalConnectionStringSettings.Name, newConnectionStringValue)
                   : new ConnectionStringSettings(originalConnectionStringSettings.Name, newConnectionStringValue, originalConnectionStringSettings.ProviderName);
        }

        private bool IsSupportedProvider(string providerName) => string.IsNullOrEmpty(providerName) || _connectionStringProviderComparer.Equals(providerName, ExpectedOrDefaultProvider);

        private ILookup<string, IDictionary<string, string>> GetOverrideAppSettingKeysByConnectionStringPartNameByConnectionStringName()
        {
            var overrides =
                from appSettingKey in _overrideSource.Keys
                where _adaptationStrategy.IsConnectionStringOverridingAppSettingKey(appSettingKey)
                let connectionStringName = _adaptationStrategy.ConvertToConnectionStringNameFromAppSettingKey(appSettingKey)
                let connectionStringPartName =
                    _adaptationStrategy.ConvertToConnectionStringPartNameFromAppSettingKey(appSettingKey)
                select new { connectionStringName, connectionStringPartName, appSettingKey };

            var overrideAppSettingKeysByConnectionStringPartNameByConnectionStringName = overrides
                .GroupBy(
                    os => os.connectionStringName,
                    os => new { os.connectionStringPartName, os.appSettingKey },
                    (gk, gv) => new KeyValuePair<string, IDictionary<string, string>>(gk, gv.ToDictionary(kvp => kvp.connectionStringPartName, kvp => kvp.appSettingKey, _connectionStringPartNameComparer)))
                .ToLookup(os => os.Key, os => os.Value, _connectionStringNameComparer);
            return overrideAppSettingKeysByConnectionStringPartNameByConnectionStringName;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
