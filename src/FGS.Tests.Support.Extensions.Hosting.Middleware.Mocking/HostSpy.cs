using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace FGS.Tests.Support.Extensions.Hosting.Middleware.Mocking
{
    /// <summary>
    /// Provides a singleton-bound mechanism for a deferred resolution of <see cref="IHost"/> to have its invocations spied upon.
    /// </summary>
    /// <remarks>Not thread-safe.</remarks>
    /// <example>In your test fixture, register this class with your dependency resolution mechanism of choice. Then, be sure to call <see cref="Reset"/> between tests.</example>
    public sealed class HostSpy : IHost
    {
        /// <summary>
        /// Gets the number of times that <see cref="IHost.StartAsync(CancellationToken)"/> has been invoked.
        /// </summary>
        public static int StartAsyncCallCount { get; private set; } = 0;

        /// <summary>
        /// Gets the number of times that <see cref="IHost.StopAsync(CancellationToken)"/> has been invoked.
        /// </summary>
        public static int StopAsyncCallCount { get; private set; } = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="HostSpy"/> class.
        /// </summary>
        /// <param name="services">The services that should be exposed by the <see cref="IHost"/>.</param>
        public HostSpy(IServiceProvider services)
        {
            Services = services;
        }

        void IDisposable.Dispose()
        {
        }

        Task IHost.StartAsync(CancellationToken cancellationToken)
        {
            StartAsyncCallCount++;
            return Task.CompletedTask;
        }

        Task IHost.StopAsync(CancellationToken cancellationToken)
        {
            StopAsyncCallCount++;
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public IServiceProvider Services { get; private set; }

        /// <summary>
        /// Resets the invocation counters.
        /// </summary>
        public static void Reset()
        {
            StartAsyncCallCount = 0;
            StopAsyncCallCount = 0;
        }
    }
}
