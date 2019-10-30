using System;
using System.ComponentModel;

using FGS.FaultHandling.Abstractions.Retry;

namespace FGS.FaultHandling.Predicates.Win32
{
    public sealed class Win32ExceptionRetryPredicate : IExceptionRetryPredicate
    {
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
