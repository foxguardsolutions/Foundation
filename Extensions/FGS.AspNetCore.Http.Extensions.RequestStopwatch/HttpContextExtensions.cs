using System;
using System.Diagnostics;

using Microsoft.AspNetCore.Http;

namespace FGS.AspNetCore.Http.Extensions.RequestStopwatch
{
    public static class HttpContextExtensions
    {
        private static readonly string RequestStopwatchKey =
            typeof(RequestStopwatchMiddleware).FullName + ".RequestStopwatch";

        public static TimeSpan? GetRequestStopwatchElapsed(this HttpContext self)
        {
            return (self.Items[RequestStopwatchKey] as Stopwatch)?.Elapsed;
        }

        internal static Stopwatch EnsureHasRequestStopwatch(this HttpContext self)
        {
            if (self.Items[RequestStopwatchKey] is Stopwatch)
            {
                return (Stopwatch)self.Items[RequestStopwatchKey];
            }

            var stopwatch = Stopwatch.StartNew();
            self.Items[RequestStopwatchKey] = stopwatch;

            return stopwatch;
        }
    }
}