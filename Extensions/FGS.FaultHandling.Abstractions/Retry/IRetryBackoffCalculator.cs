using System;

namespace FGS.FaultHandling.Abstractions.Retry
{
    public interface IRetryBackoffCalculator
    {
        TimeSpan CalculateBackoff(int retryAttempt);
    }
}