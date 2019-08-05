using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace FGS.AspNetCore.Http.Extensions.RequestStopwatch
{
    public class RequestStopwatchMiddleware : IMiddleware
    {
        private readonly ILogger _logger;

        public RequestStopwatchMiddleware(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger(GetType());
        }

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
