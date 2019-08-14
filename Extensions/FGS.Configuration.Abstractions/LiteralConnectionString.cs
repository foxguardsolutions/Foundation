namespace FGS.Configuration.Abstractions
{
    public class LiteralConnectionString : IConnectionString
    {
        public LiteralConnectionString(string connectionString, string providerName)
        {
            ConnectionString = connectionString;
            ProviderName = providerName;
        }

        public string ConnectionString { get; }

        public string ProviderName { get; }
    }
}
