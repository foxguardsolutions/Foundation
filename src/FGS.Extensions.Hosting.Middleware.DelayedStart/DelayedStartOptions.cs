using System;

namespace FGS.Extensions.Hosting.Middleware.DelayedStart
{

    /// <summary>
    /// Provides configuration options for <see cref="DelayedStartHostingMiddleware"/>.
    /// </summary>
    public class DelayedStartOptions
    {
        /// <summary>
        /// Gets or sets the amount of time that startup will be delayed.
        /// </summary>
        public TimeSpan Delay { get; set; } = TimeSpan.Zero;
    }
}
