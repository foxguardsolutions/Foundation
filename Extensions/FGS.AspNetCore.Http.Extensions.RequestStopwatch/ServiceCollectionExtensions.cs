using Microsoft.Extensions.DependencyInjection;

namespace FGS.AspNetCore.Http.Extensions.RequestStopwatch
{
    /// <summary>
    /// Extends <see cref="IServiceCollection"/> with functionality to register middleware that wraps web requests with stopwatches.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers <see cref="RequestStopwatchMiddleware"/> with <paramref name="services"/>.
        /// </summary>
        /// <remarks>
        /// Remember to use <see cref="AppBuilderExtensions.UseRequestStopwatch"/> to register the middleware with the request pipeline.
        /// </remarks>
        /// <example>
        /// <code>
        ///   services.AddRequestStopwatch();
        /// </code>
        /// </example>
        public static void AddRequestStopwatch(this IServiceCollection services)
        {
            services.AddScoped<RequestStopwatchMiddleware>();
        }
    }
}
