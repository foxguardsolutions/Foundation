namespace FGS.Pump.Configuration.Patterns.Specialized
{
    public interface IAppSettingsOverriddenSqlServerConnectionStringsAdaptationStrategy
    {
        bool IsConnectionStringOverridingAppSettingKey(string appSettingKey);
        string ConvertToConnectionStringNameFromAppSettingKey(string appSettingKey);
        string ConvertToConnectionStringPartNameFromAppSettingKey(string appSettingKey);
    }
}