namespace FGS.FaultHandling.Abstractions.Retry
{
    /// <summary>
    /// Represents a parameter-less factory of <see cref="IRetryPolicy"/> instances.
    /// </summary>
    public interface IRetryPolicyCoordinator
    {
        /// <summary>
        /// Creates an instance of <see cref="IRetryPolicy"/>.
        /// </summary>
        /// <returns>Returns an <see cref="IRetryPolicy"/>.</returns>
        IRetryPolicy RequestPolicy();
    }
}
