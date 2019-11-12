using System;
using System.Threading;
using System.Threading.Tasks;

using FGS.Extensions.Hosting.Middleware.Abstractions;

using Microsoft.Extensions.DependencyInjection;

namespace FGS.Extensions.Hosting.Middleware
{
    /// <summary>
    /// An implementation of <see cref="IHostingMiddleware"/> whose purpose is to defer implementation to a transiently resolved
    /// instance of <typeparamref name="THostingMiddleware"/>, which is acquired via an <see cref="IServiceScope"/>.
    /// </summary>
    /// <typeparam name="THostingMiddleware">The type of <see cref="IHostingMiddleware"/> that will be resolved and used as the underlying implementation.</typeparam>
    internal sealed class ServiceScopeResolvedHostingMiddlewareDecoraptor<THostingMiddleware> : IHostingMiddleware
        where THostingMiddleware : IHostingMiddleware
    {
        private readonly Func<IServiceScope> _serviceScopeFactory;
        private readonly Func<IServiceProvider, THostingMiddleware> _hostingMiddlewareFactory;

        internal ServiceScopeResolvedHostingMiddlewareDecoraptor(Func<IServiceScope> serviceScopeFactory, Func<IServiceProvider, THostingMiddleware> hostingMiddlewareFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _hostingMiddlewareFactory = hostingMiddlewareFactory;
        }

        Task IHostingMiddleware.StartAsync(Func<Task> next, CancellationToken cancellationToken)
        {
            using var serviceScope = _serviceScopeFactory();
            var middleware = _hostingMiddlewareFactory(serviceScope.ServiceProvider);
            return middleware.StartAsync(next, cancellationToken);
        }

        Task IHostingMiddleware.StopAsync(Func<Task> next, CancellationToken cancellationToken)
        {
            using var serviceScope = _serviceScopeFactory();
            var middleware = _hostingMiddlewareFactory(serviceScope.ServiceProvider);
            return middleware.StopAsync(next, cancellationToken);
        }
    }
}
