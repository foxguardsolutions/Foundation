using System;

using FGS.FaultHandling.Abstractions.Retry;

namespace FGS.FaultHandling.Primitives.Retry
{
    /// <summary>
    /// Calculates the duration of a wait between retries of an operation, using an exponential formula based on the retry attempt ordinal.
    /// </summary>
    public sealed class ExponentialRetryBackoffCalculator : IRetryBackoffCalculator
    {
        /// <summary>
        /// Calculates the duration of a wait between retries of an operation, using an exponential formula based on the retry attempt ordinal.
        /// </summary>
        /// <param name="retryAttempt">The ordinal of which retry attempt the backoff should be calculated for.</param>
        /// <returns>A <see cref="TimeSpan"/> representing the amount of time that should be waited before the next retry attempt.</returns>
        public TimeSpan CalculateBackoff(int retryAttempt) => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt - 1));
    }
}
