using System;

namespace FGS.Pump.FaultHandling.Retry
{
    internal sealed class ExponentialRetryBackoffCalculator : IRetryBackoffCalculator
    {
        public TimeSpan CalculateBackoff(int retryAttempt) => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt - 1));
    }
}