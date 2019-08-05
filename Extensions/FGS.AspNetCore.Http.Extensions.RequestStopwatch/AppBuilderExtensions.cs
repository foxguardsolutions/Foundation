using Microsoft.AspNetCore.Builder;

namespace FGS.AspNetCore.Http.Extensions.RequestStopwatch
{
    public static class AppBuilderExtensions
    {
        public static void UseRequestStopwatch(this IApplicationBuilder app)
        {
            app.UseMiddleware<RequestStopwatchMiddleware>();
        }
    }
}
