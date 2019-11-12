using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace FGS.AspNetCore.Http.Extensions.RequestStopwatch
{
    /// <summary>
    /// Request middleware that creates and manages a stopwatch to time the current request.
    /// </summary>
    /// <remarks>
    /// The current value of the current request's stopwatch can be retrieved using <see cref="HttpContextExtensions"/>.
    /// </remarks>
    public sealed class RequestStopwatchMiddleware : IMiddleware
    {
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestStopwatchMiddleware"/> class.
        /// </summary>
        public RequestStopwatchMiddleware(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger(GetType());
        }

        /// <inheritdoc cref="IMiddleware.InvokeAsync"/>
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            _logger.LogDebug("Starting stopwatch");
            var stopwatch = context.EnsureHasRequestStopwatch();
            try
            {
                await next(context).ConfigureAwait(false);
            }
            finally
            {
                stopwatch.Stop();
                _logger.LogInformation("Stopwatch stopped after {Request_DurationMs}ms", stopwatch.ElapsedMilliseconds);
            }
        }
    }
}
