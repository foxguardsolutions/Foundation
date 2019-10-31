using System;
using System.ComponentModel;

using FGS.FaultHandling.Abstractions.Retry;

namespace FGS.FaultHandling.Predicates.Win32
{
    /// <summary>
    /// Indicates whether the given exception, or its inner exception, is a <see cref="Win32Exception"/>,
    /// for which we want to attempt to retry the operation.
    /// </summary>
    public sealed class Win32ExceptionRetryPredicate : IExceptionRetryPredicate
    {
        /// <inheritdoc />
        public bool ShouldRetry(Exception ex)
        {
            while (true)
            {
                if (ex is Win32Exception) return true;

                if (ex.InnerException == null) return false;

                ex = ex.InnerException;
            }
        }
    }
}
