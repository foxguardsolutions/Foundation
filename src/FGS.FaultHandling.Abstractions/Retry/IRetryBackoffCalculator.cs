using System;

namespace FGS.FaultHandling.Abstractions.Retry
{
    /// <summary>
    /// Represents a calculation for how long the retry of an operation should be delayed based on which retry attempt ordinal it is.
    /// </summary>
    public interface IRetryBackoffCalculator
    {
        /// <summary>
        /// Determines how long the retry of an operation should be delayed based on which retry attempt ordinal it is.
        /// </summary>
        /// <param name="retryAttempt">The ordinal of which retry attempt the backoff should be calculated for.</param>
        /// <returns>A <see cref="TimeSpan"/> representing the amount of time that should be waited before the next retry attempt.</returns>
        TimeSpan CalculateBackoff(int retryAttempt);
    }
}
