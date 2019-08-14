using Microsoft.Extensions.Configuration;

namespace FGS.Configuration.Abstractions
{
    public static class ConfigurationExtensions
    {
        public static IConnectionString GetConnectionStringAndProvider(this IConfiguration configuration, string name)
        {
            var section = configuration.GetSection("ConnectionStrings");
            return new LiteralConnectionString(section[name], section[name + "_ProviderName"]);
        }
    }
}
