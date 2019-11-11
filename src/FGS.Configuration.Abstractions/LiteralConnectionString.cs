namespace FGS.Configuration.Abstractions
{
    /// <summary>
    /// Represents a pair of connection string and provider name, whose literal values are known at instantiation.
    /// </summary>
    public class LiteralConnectionString : IConnectionString
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LiteralConnectionString"/> class.
        /// </summary>
        /// <param name="connectionString">The value of the connection string.</param>
        /// <param name="providerName">The name of the ADO.NET provider to be used for creating connections for the given connection string value.</param>
        public LiteralConnectionString(string connectionString, string providerName)
        {
            ConnectionString = connectionString;
            ProviderName = providerName;
        }

        /// <inheritdoc />
        public string ConnectionString { get; }

        /// <inheritdoc />
        public string ProviderName { get; }
    }
}
