using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;

namespace FGS.AspNetCore.Http.Extensions.AWS.ALB
{
    /// <summary>
    /// Extends <see cref="IApplicationBuilder"/> with functionality to configure middleware that forwards proxied HTTP headers.
    /// </summary>
    public static class AwsAlbApplicationBuilderExtensions
    {
        /// <summary>
        /// Forwards proxied headers on to current request, for cases when proxy is expected to be an AWS Application Load Balancer.
        /// </summary>
        /// <param name="app">The application being configured.</param>
        /// <param name="tlsOffloaded">Indicates whether or not the AWS ALB is configured to offload TLS, and therefor whether or not to expect the <c>X-Forwarded-Proto</c> header. The default is <c>true</c>.</param>
        /// <param name="hostIsRewritten">Indicates whether or not the AWS ALB is configured to rewrite the <c>Host</c> header, and therefor whether or not to expect the <c>X-Forwarded-Host</c> header. The default is <c>false</c>.</param>
        /// <returns>An instance of <see cref="IApplicationBuilder"/> so that configuration calls can be chained.</returns>
        /// <remarks>
        /// <para>
        ///     This adapter on top of <see cref="ForwardedHeadersExtensions.UseForwardedHeaders(IApplicationBuilder,ForwardedHeadersOptions)"/> is needed when the application is running behind an AWS Application Load Balancer and the consumer wishes to leverage forwarded headers.
        ///     It is notable that the underlying middleware does not activate unless it is configured in a way that precisely matches incoming requests, which is tricky because AWS ALBs only forward headers based on how they themselves are configured.
        ///     This helper bridges the gap between the two by allowing a consumer to express expected headers in terms relating to how the AWS ALB is configured.
        /// </para>
        /// <para>
        ///     ⚠WARNING: The implementation of this explicitly configures the underlying middleware to trust the forwarding headers regardless as to network source of the connection. This could pose a spoofing risk on untrusted networks.
        /// </para>
        /// </remarks>
        /// <example>
        /// <code>
        ///   app.UseForwardedHeadersFromAwsApplicationLoadBalancer();
        /// </code>
        /// </example>
        public static IApplicationBuilder UseForwardedHeadersFromAwsApplicationLoadBalancer(this IApplicationBuilder app, bool tlsOffloaded = true, bool hostIsRewritten = false)
        {
            var forwardedHeaders = ForwardedHeaders.XForwardedFor;

            if (tlsOffloaded)
            {
                forwardedHeaders |= ForwardedHeaders.XForwardedProto;
            }

            if (hostIsRewritten)
            {
                forwardedHeaders |= ForwardedHeaders.XForwardedHost;
            }

            var options = new ForwardedHeadersOptions()
            {
                ForwardedHeaders = forwardedHeaders,
            };

            // We have to explicitly clear these. They need to be empty in order for `ForwardedHeadersMiddleware` to skip IP validation.
            options.KnownNetworks.Clear();
            options.KnownProxies.Clear();

            return app.UseForwardedHeaders(options);
        }
    }
}
