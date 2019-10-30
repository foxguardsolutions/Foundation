using System;

namespace FGS.FaultHandling.Abstractions.Retry
{
    public interface IExceptionRetryPredicate
    {
        bool ShouldRetry(Exception ex);
    }
}