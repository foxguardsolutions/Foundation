using Microsoft.AspNetCore.Builder;

namespace FGS.AspNetCore.Http.Extensions.RequestStopwatch
{
    /// <summary>
    /// Extends <see cref="IApplicationBuilder"/> with functionality to configure middleware that will time web requests with a stopwatch.
    /// </summary>
    public static class AppBuilderExtensions
    {
        /// <summary>
        /// Adds <see cref="RequestStopwatchMiddleware"/> to the request pipeline.
        /// </summary>
        /// <remarks>
        /// Make sure <see cref="RequestStopwatchMiddleware"/> is resolvable from dependency injection.
        /// Use <see cref="ServiceCollectionExtensions.AddRequestStopwatch"/> if needed.
        /// </remarks>
        /// <example>
        /// <code>
        ///   app.UseRequestStopwatch();
        /// </code>
        /// </example>
        public static void UseRequestStopwatch(this IApplicationBuilder app)
        {
            app.UseMiddleware<RequestStopwatchMiddleware>();
        }
    }
}
