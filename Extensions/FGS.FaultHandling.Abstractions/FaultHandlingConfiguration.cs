namespace FGS.FaultHandling.Abstractions
{
    /// <summary>
    /// Fault-handling configuration.
    /// </summary>
    public sealed class FaultHandlingConfiguration
    {
        /// <summary>
        /// Gets or sets the maximum number of times an operation will be retried.
        /// </summary>
        public int MaxRetries { get; set; }
    }
}
