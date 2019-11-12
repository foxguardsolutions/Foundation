using System;
using System.Threading;
using System.Threading.Tasks;

using FGS.Extensions.Hosting.Middleware.Abstractions;

using Microsoft.Extensions.Hosting;

namespace FGS.Extensions.Hosting.Middleware
{
    /// <summary>
    /// An implementation of <see cref="IHost"/> that decorates an underlying instance with interceptor behavior represented
    /// by a given instance of <see cref="IHostingMiddleware"/>.
    /// </summary>
    internal sealed class MiddlewareApplyingHostDecorator : IHost
    {
        private readonly IHost _decorated;
        private readonly IHostingMiddleware _hostingMiddleware;

        internal MiddlewareApplyingHostDecorator(IHost decorated, IHostingMiddleware hostingMiddleware)
        {
            _decorated = decorated;
            _hostingMiddleware = hostingMiddleware;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
        }

        Task IHost.StartAsync(CancellationToken cancellationToken) =>
            _hostingMiddleware.StartAsync(() => _decorated.StartAsync(cancellationToken), cancellationToken);

        Task IHost.StopAsync(CancellationToken cancellationToken) =>
            _hostingMiddleware.StopAsync(() => _decorated.StopAsync(cancellationToken), cancellationToken);

        IServiceProvider IHost.Services => _decorated.Services;
    }
}
