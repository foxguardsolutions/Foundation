using System;
using System.Threading;
using System.Threading.Tasks;
using FGS.Extensions.Hosting.Middleware.Abstractions;

namespace FGS.Extensions.Hosting.Middleware.DelayedStart
{
    /// <summary>
    /// Implements a configurable startup delay as <see cref="IHostingMiddleware"/>.
    /// </summary>
    public sealed class DelayedStartHostingMiddleware : IHostingMiddleware
    {
        private DelayedStartOptions _options;

        /// <summary>
        /// Intializes a new instance of the <see cref="DelayedStartHostingMiddleware"/> class, using the provided
        /// <paramref name="options"/> as its configuration.
        /// </summary>
        /// <param name="options">The configuration to use.</param>
        public DelayedStartHostingMiddleware(DelayedStartOptions options)
        {
            _options = options;
        }

        async Task IHostingMiddleware.StartAsync(Func<Task> next, CancellationToken cancellationToken)
        {
            var delay = _options.Delay;
            if (delay > TimeSpan.Zero)
            {
                await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
                await next().ConfigureAwait(false);
            } 
            else
            {
                await next().ConfigureAwait(false);
            }
        }

        Task IHostingMiddleware.StopAsync(Func<Task> next, CancellationToken cancellationToken) =>
            next();
    }
}
