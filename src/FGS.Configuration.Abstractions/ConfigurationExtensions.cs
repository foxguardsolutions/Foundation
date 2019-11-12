using Microsoft.Extensions.Configuration;

namespace FGS.Configuration.Abstractions
{
    /// <summary>
    /// Extends <see cref="IConfiguration"/> with functionality to read a <see cref="IConnectionString"/>.
    /// </summary>
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Reads a <see cref="IConnectionString"/> by reading from a section named <value>ConnectionStrings</value> in the given <paramref name="configuration"/>.
        /// The connection string value is located at a key with the name <paramref name="name"/>, whereas the ADO.NET provider name is sought
        /// at the key with the same name but suffixed with <value>_ProviderName</value>.
        /// </summary>
        /// <param name="configuration">The configuration from which to read the <see cref="IConnectionString"/>. Must contain a section named <value>ConnectionStrings</value>.</param>
        /// <param name="name">The name of the connection string that is sought.</param>
        /// <returns>The retrieved <see cref="IConnectionString"/>.</returns>
        public static IConnectionString GetConnectionStringAndProvider(this IConfiguration configuration, string name)
        {
            var section = configuration.GetSection("ConnectionStrings");
            return new LiteralConnectionString(section[name], section[name + "_ProviderName"]);
        }
    }
}
