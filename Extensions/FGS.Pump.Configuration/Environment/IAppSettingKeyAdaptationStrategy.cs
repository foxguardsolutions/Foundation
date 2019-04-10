namespace FGS.Pump.Configuration.Environment
{
    public interface IAppSettingsKeyAdaptationStrategy
    {
        bool IsAppSettingUnderlyingKey(string underlyingKey);

        string ToAppSettingName(string underlyingKey);
    }
}
