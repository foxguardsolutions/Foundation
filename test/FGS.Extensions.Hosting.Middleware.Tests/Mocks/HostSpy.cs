using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace FGS.Extensions.Hosting.Middleware.Tests.Mocks
{
    public sealed class HostSpy : IHost
    {
        public static int StartAsyncCallCount = 0;
        public static int StopAsyncCallCount = 0;

        public HostSpy(IServiceProvider services)
        {
            Services = services;
        }

        public void Dispose()
        {
        }

        public Task StartAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            StartAsyncCallCount++;
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            StopAsyncCallCount++;
            return Task.CompletedTask;
        }

        public IServiceProvider Services { get; }

        public static void Reset()
        {
            StartAsyncCallCount = 0;
            StopAsyncCallCount = 0;
        }
    }
}
