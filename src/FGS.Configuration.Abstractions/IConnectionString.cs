namespace FGS.Configuration.Abstractions
{
    /// <summary>
    /// Represents a pair of connection string and provider name.
    /// </summary>
    public interface IConnectionString
    {
        /// <summary>
        /// Gets the value of the connection string.
        /// </summary>
        string ConnectionString { get; }

        /// <summary>
        /// Gets the name of the ADO.NET provider to be used for creating connections for the given connection string value.
        /// </summary>
        string ProviderName { get; }
    }
}
