namespace FGS.Pump.Configuration.Environment
{
    public interface ISplitConnectionStringAdaptationStrategy
    {
        bool IsConnectionStringValueUnderlyingKey(string key);
        bool IsConnectionStringProviderUnderlyingKey(string key);

        string ConvertToConnectionStringNameFromValueUnderlyingKey(string key);
        string ConvertToConnectionStringNameFromProviderUnderlyingKey(string key);
    }
}
