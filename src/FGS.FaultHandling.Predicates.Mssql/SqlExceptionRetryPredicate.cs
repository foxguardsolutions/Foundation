using System;
using System.Linq;

using FGS.FaultHandling.Abstractions.Retry;

#if NET472 || NETSTANDARD2_0
using System.Data.SqlClient;
#elif NETSTANDARD2_1 || NETCOREAPP3_0
using Microsoft.Data.SqlClient;
#endif

namespace FGS.FaultHandling.Predicates.Mssql
{
    /// <summary>
    /// Indicates whether the given exception, or its inner exception, is a <see cref="SqlException"/> with a
    /// <see cref="SqlError"/> for which we want to attempt to retry the operation.
    /// </summary>
    /// <remarks>
    /// SqlErrors with a class greater than or equal to 17 indicate that a hardware or software error occurred.
    /// For such errors we would want to attempt to retry the operation. SqlErrors with a class less than 17 are
    /// used to indicate that the user needs to correct something before the operation can succeed, and so we do
    /// not want to retry if there are any of those.
    /// See https://msdn.microsoft.com/en-us/library/system.data.sqlclient.sqlerror.class(v=vs.110).aspx.
    /// </remarks>
    public sealed class SqlExceptionRetryPredicate : IExceptionRetryPredicate
    {
        private const int SqlHardwareOrSoftwareErrorClassLowerBound = 17;

        public bool ShouldRetry(Exception ex)
        {
            while (true)
            {
                var sqlException = ex as SqlException;

                if (sqlException == null)
                {
                    if (ex.InnerException == null) return false;

                    ex = ex.InnerException;
                    continue;
                }

                var sqlErrors = sqlException.Errors.Cast<SqlError>();

                return sqlErrors.Any(error => error.Class >= SqlHardwareOrSoftwareErrorClassLowerBound);
            }
        }
    }
}
