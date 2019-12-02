using FGS.Extensions.Hosting.Middleware.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FGS.Tests.Support.Extensions.Hosting.Middleware.Mocking
{
    /// <summary>
    /// Implements an <see cref="IHostingMiddleware"/> that DOES continue execution into nested middleware.
    /// </summary>
    public sealed class HostCallingHostingMiddleware : IHostingMiddleware
    {
        Task IHostingMiddleware.StartAsync(Func<Task> next, CancellationToken cancellationToken) =>
            next();

        Task IHostingMiddleware.StopAsync(Func<Task> next, CancellationToken cancellationToken) =>
            next();
    }
}
