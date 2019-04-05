using System;

namespace FGS.Pump.FaultHandling.Retry
{
    public interface IExceptionRetryPredicate
    {
        bool ShouldRetry(Exception ex);
    }
}