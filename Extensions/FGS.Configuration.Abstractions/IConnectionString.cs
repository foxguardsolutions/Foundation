namespace FGS.Configuration.Abstractions
{
    public interface IConnectionString
    {
        string ConnectionString { get; }

        string ProviderName { get; }
    }
}
