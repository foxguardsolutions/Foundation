using System;

using FGS.Extensions.Hosting.Middleware.Abstractions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FGS.Extensions.Hosting.Middleware
{
    /// <summary>
    /// Extends <see cref="IHostBuilder"/> with functionality for configuring the usage of instances of <see cref="IHostingMiddleware"/> to intercept calls
    /// to the later-built <see cref="IHost"/>.
    /// </summary>
    /// <example>
    /// <code>
    /// public static IWebHostBuilder CreateWebHostBuilder(string[] args) =&gt;
    ///     WebHost.CreateDefaultBuilder(args)
    ///         .UseStartup&lt;Startup&gt;()
    ///         .UseHostingMiddleware&lt;MyHostingMiddleware&gt;();
    /// </code>
    /// </example>
    public static class HostBuilderExtensions
    {
        /// <summary>
        /// Adds hosting middleware functionality to the web host startup &amp; graceful shutdown process, relying on an instance of
        /// <typeparamref name="THostingMiddleware"/> to provide interceptor behavior.
        /// </summary>
        /// <param name="hostBuilder">An instance of the program initialization abstraction that is to be modified.</param>
        /// <param name="hostingMiddlewareFactory">Optionally creates or retrieves an instance of <typeparamref name="THostingMiddleware"/> to use.
        /// If not specified, an instance will be requested from the <see cref="IServiceProvider"/>.</param>
        /// <typeparam name="THostingMiddleware">The type of <see cref="IHostingMiddleware"/> that will be used to intercept web host startup &amp; graceful shutdown.</typeparam>
        /// <returns>A program initialization abstraction that has been augmented to apply a <see cref="IHostingMiddleware"/> to the eventually-created <see cref="IHost"/>.</returns>
        /// <remarks>Multiple calls to this will apply multiple layers of the hosting middleware stack, each one specific to that invocation and desired middleware.</remarks>
        public static IHostBuilder UseHostingMiddleware<THostingMiddleware>(this IHostBuilder hostBuilder, Func<IServiceProvider, THostingMiddleware> hostingMiddlewareFactory = null)
            where THostingMiddleware : IHostingMiddleware
        {
            hostingMiddlewareFactory ??= (sp) => sp.GetRequiredService<THostingMiddleware>();

            IHost CreateHostDecorator(IHost decorated)
            {
                var hostingMiddlewareDecoraptor = new ServiceScopeResolvedHostingMiddlewareDecoraptor<THostingMiddleware>(() => decorated.Services.CreateScope(), hostingMiddlewareFactory);

                return new MiddlewareApplyingHostDecorator(decorated, hostingMiddlewareDecoraptor);
            }

            return new DecoratorApplyingHostBuilderDecorator(hostBuilder, CreateHostDecorator);
        }
    }
}
