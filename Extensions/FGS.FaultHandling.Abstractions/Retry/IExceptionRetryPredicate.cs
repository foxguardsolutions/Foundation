using System;

namespace FGS.FaultHandling.Abstractions.Retry
{
    /// <summary>
    /// Represents conditional logic as to whether or not an operation that failed from a given exception should be retried.
    /// </summary>
    public interface IExceptionRetryPredicate
    {
        /// <summary>
        /// Determines whether or not an operation that failed from <paramref name="ex"/> should be retried.
        /// </summary>
        /// <param name="ex">The <see cref="Exception"/> that operation failed with.</param>
        /// <returns><see langword="true"/> if the operation should be retried, <see langword="false"/> if it should not be retried.</returns>
        bool ShouldRetry(Exception ex);
    }
}
