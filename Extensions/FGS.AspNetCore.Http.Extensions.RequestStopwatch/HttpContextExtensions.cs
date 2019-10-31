using System;
using System.Diagnostics;

using Microsoft.AspNetCore.Http;

namespace FGS.AspNetCore.Http.Extensions.RequestStopwatch
{
    /// <summary>
    /// Extends <see cref="HttpContext"/> with functionality to query the current web request's stopwatch.
    /// </summary>
    public static class HttpContextExtensions
    {
        private static readonly string RequestStopwatchKey =
            typeof(RequestStopwatchMiddleware).FullName + ".RequestStopwatch";

        /// <summary>
        /// Gets the current <see cref="Stopwatch.Elapsed"/> value of the <see cref="Stopwatch"/> that has been embedded
        /// into the <see cref="HttpContext"/> of the current request by an instance of <see cref="RequestStopwatchMiddleware"/>.
        /// </summary>
        /// <param name="self">The <see cref="HttpContext"/> containing the <see cref="Stopwatch"/> to be queried.</param>
        /// <remarks>Requires that <paramref name="self"/> contains a well-known instance of <see cref="Stopwatch"/>, such as
        /// that introduced and managed by a <see cref="RequestStopwatchMiddleware"/>.</remarks>
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
