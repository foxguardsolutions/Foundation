using System;
using System.Threading;
using System.Threading.Tasks;

namespace FGS.Extensions.Hosting.Middleware.Abstractions
{
    /// <summary>
    /// Defines middleware that can be added to the application's web hosting pipeline as a means to asynchronously intercept
    /// startup and graceful shutdowns.
    /// </summary>
    public interface IHostingMiddleware
    {
        /// <summary>
        /// Represents functionality that runs before and/or after the underlying web host starts listening on the configured address.
        /// </summary>
        /// <param name="next">The delegate representing the remaining middleware in the web hosting pipeline.</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        Task StartAsync(Func<Task> next, CancellationToken cancellationToken = default);

        /// <summary>
        /// Represents functionality that runs before and/or after attempting to gracefully stop the underlying web host.
        /// </summary>
        /// <param name="next">The delegate representing the remaining middleware in the web hosting pipeline.</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        Task StopAsync(Func<Task> next, CancellationToken cancellationToken = default);
    }
}
