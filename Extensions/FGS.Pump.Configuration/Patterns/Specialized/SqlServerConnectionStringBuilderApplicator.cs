using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace FGS.Pump.Configuration.Patterns.Specialized
{
    public class SqlServerConnectionStringBuilderApplicator : ISqlServerConnectionStringBuilderApplicator
    {
        private readonly IDictionary<string, Action<SqlConnectionStringBuilder, string>> _updaters = new Dictionary<string, Action<SqlConnectionStringBuilder, string>>()
        {
            { "ApplicationIntent", (csb, v) => csb.ApplicationIntent = (ApplicationIntent)Enum.Parse(typeof(ApplicationIntent), v) },
            { "ApplicationName", (csb, v) => csb.ApplicationName = v },
            { "AsynchronousProcessing", (csb, v) => csb.AsynchronousProcessing = bool.Parse(v) },
            { "AttachDBFilename", (csb, v) => csb.AttachDBFilename = v },
            { "Authentication", (csb, v) => csb.Authentication = (SqlAuthenticationMethod)Enum.Parse(typeof(SqlAuthenticationMethod), v) },
            { "ColumnEncryptionSetting", (csb, v) => csb.ColumnEncryptionSetting = (SqlConnectionColumnEncryptionSetting)Enum.Parse(typeof(SqlConnectionColumnEncryptionSetting), v) },
            { "ConnectRetryCount", (csb, v) => csb.ConnectRetryCount = int.Parse(v) },
            { "ConnectRetryInterval", (csb, v) => csb.ConnectRetryInterval = int.Parse(v) },
            { "ConnectTimeout", (csb, v) => csb.ConnectTimeout = int.Parse(v) },
            { "ContextConnection", (csb, v) => csb.ContextConnection = bool.Parse(v) },
            { "CurrentLanguage", (csb, v) => csb.CurrentLanguage = v },
            { "DataSource", (csb, v) => csb.DataSource = v },
            { "Encrypt", (csb, v) => csb.Encrypt = bool.Parse(v) },
            { "Enlist", (csb, v) => csb.Enlist = bool.Parse(v) },
            { "FailoverPartner", (csb, v) => csb.FailoverPartner = v },
            { "InitialCatalog", (csb, v) => csb.InitialCatalog = v },
            { "IntegratedSecurity", (csb, v) => csb.IntegratedSecurity = bool.Parse(v) },
            { "LoadBalanceTimeout", (csb, v) => csb.LoadBalanceTimeout = int.Parse(v) },
            { "MaxPoolSize", (csb, v) => csb.MaxPoolSize = int.Parse(v) },
            { "MinPoolSize", (csb, v) => csb.MinPoolSize = int.Parse(v) },
            { "MultipleActiveResultSets", (csb, v) => csb.MultipleActiveResultSets = bool.Parse(v) },
            { "MultiSubnetFailover", (csb, v) => csb.MultiSubnetFailover = bool.Parse(v) },
            { "NetworkLibrary", (csb, v) => csb.NetworkLibrary = v },
            { "PacketSize", (csb, v) => csb.PacketSize = int.Parse(v) },
            { "Password", (csb, v) => csb.Password = v },
            { "PersistSecurityInfo", (csb, v) => csb.PersistSecurityInfo = bool.Parse(v) },
            { "PoolBlockingPeriod", (csb, v) => csb.PoolBlockingPeriod = (PoolBlockingPeriod)Enum.Parse(typeof(PoolBlockingPeriod), v) },
            { "Pooling", (csb, v) => csb.Pooling = bool.Parse(v) },
            { "Replication", (csb, v) => csb.Replication = bool.Parse(v) },
            { "TransactionBinding", (csb, v) => csb.TransactionBinding = v },
            { "TransparentNetworkIPResolution", (csb, v) => csb.TransparentNetworkIPResolution = bool.Parse(v) },
            { "TrustServerCertificate", (csb, v) => csb.TrustServerCertificate = bool.Parse(v) },
            { "TypeSystemVersion", (csb, v) => csb.TypeSystemVersion = v },
            { "UserID", (csb, v) => csb.UserID = v },
            { "UserInstance", (csb, v) => csb.UserInstance = bool.Parse(v) },
            { "WorkstationID", (csb, v) => csb.WorkstationID = v }
        };

        public void Apply(SqlConnectionStringBuilder connectionStringBuilder, string key, string value) => _updaters[key](connectionStringBuilder, value);
    }
}
