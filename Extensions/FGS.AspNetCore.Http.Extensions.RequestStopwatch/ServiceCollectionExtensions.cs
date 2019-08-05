using System;

using Microsoft.Extensions.DependencyInjection;

namespace FGS.AspNetCore.Http.Extensions.RequestStopwatch
{
    public static class ServiceCollectionExtensions
    {
        public static void AddRequestStopwatch(this IServiceCollection services)
        {
            services.AddScoped<RequestStopwatchMiddleware>();
        }
    }
}
