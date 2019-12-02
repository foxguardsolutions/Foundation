using Microsoft.Extensions.Hosting;
using System;

namespace FGS.Extensions.Hosting.Middleware.DelayedStart
{
    /// <summary>
    /// Extends <see cref="IHostBuilder"/> with functionality for configuring a delayed startup.
    /// </summary>
    /// <example>
    /// <code>
    /// public static IWebHostBuilder CreateWebHostBuilder(string[] args) =&gt;
    ///     WebHost.CreateDefaultBuilder(args)
    ///         .UseStartup&lt;Startup&gt;()
    ///         .WithDelayedStart(TimeSpan.FromSeconds(5));
    /// </code>
    /// </example>
    public static class DelayedStartHostBuilderExtensions
    {
        /// <summary>
        /// Adds hosting middleware functionality to the web host startup process which will delay the startup by the provided amount of time.
        /// </summary>
        /// <param name="hostBuilder">An instance of the program initialization abstraction that is to be modified.</param>
        /// <param name="delay">The amount of time that startup will be delayed.</param>
        /// <returns>The program initialization abstraction which has been augmented to apply a delayed start.</returns>
        /// <remarks>Multiple calls to this will apply consecutive delays.</remarks>
        public static IHostBuilder WithDelayedStart(this IHostBuilder hostBuilder, TimeSpan delay)
        {
            var options = new DelayedStartOptions()
            {
                Delay = delay
            };

            return hostBuilder.UseHostingMiddleware(ctx => new DelayedStartHostingMiddleware(options));
        }
    }
}
