using System;
using System.Threading;
using System.Threading.Tasks;
using FGS.Extensions.Hosting.Middleware.Abstractions;

namespace FGS.Extensions.Hosting.Middleware.Tests.Mocks
{
    public class HostCallingHostingMiddleware : IHostingMiddleware
    {
        public Task StartAsync(Func<Task> next, CancellationToken cancellationToken = default) =>
            next();

        public Task StopAsync(Func<Task> next, CancellationToken cancellationToken = default) =>
            next();
    }
}
