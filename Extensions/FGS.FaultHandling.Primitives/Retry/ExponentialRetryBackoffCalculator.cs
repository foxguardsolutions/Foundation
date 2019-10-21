using System;

using FGS.FaultHandling.Abstractions.Retry;

namespace FGS.FaultHandling.Primitives.Retry
{
    public sealed class ExponentialRetryBackoffCalculator : IRetryBackoffCalculator
    {
        public TimeSpan CalculateBackoff(int retryAttempt) => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt - 1));
    }
}
