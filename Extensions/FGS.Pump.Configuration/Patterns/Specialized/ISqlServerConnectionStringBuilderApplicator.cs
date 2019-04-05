using System.Data.SqlClient;

namespace FGS.Pump.Configuration.Patterns.Specialized
{
    public interface ISqlServerConnectionStringBuilderApplicator
    {
        void Apply(SqlConnectionStringBuilder connectionStringBuilder, string key, string value);
    }
}