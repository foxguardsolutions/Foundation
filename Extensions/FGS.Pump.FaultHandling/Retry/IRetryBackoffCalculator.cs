using System;

namespace FGS.Pump.FaultHandling.Retry
{
    public interface IRetryBackoffCalculator
    {
        TimeSpan CalculateBackoff(int retryAttempt);
    }
}