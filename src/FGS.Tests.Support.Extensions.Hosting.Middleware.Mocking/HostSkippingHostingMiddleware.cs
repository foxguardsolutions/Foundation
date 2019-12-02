using System;
using System.Threading;
using System.Threading.Tasks;
using FGS.Extensions.Hosting.Middleware.Abstractions;

namespace FGS.Tests.Support.Extensions.Hosting.Middleware.Mocking
{
    /// <summary>
    /// Implements an <see cref="IHostingMiddleware"/> that does NOT continue execution into nested middleware.
    /// </summary>
    public sealed class HostSkippingHostingMiddleware : IHostingMiddleware
    {
        Task IHostingMiddleware.StartAsync(Func<Task> next, CancellationToken cancellationToken) =>
            Task.CompletedTask;

        Task IHostingMiddleware.StopAsync(Func<Task> next, CancellationToken cancellationToken) =>
            Task.CompletedTask;
    }
}
