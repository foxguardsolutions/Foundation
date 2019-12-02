using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using System;

namespace FGS.Tests.Support.Extensions.Hosting.Middleware.Mocking
{
    /// <summary>
    /// Provides the ability to create a fake <see cref="IHostBuilder"/> which is robust enough for common testing scenarios.
    /// </summary>
    public static class FakedServiceScopeHostBuilderFactory
    {
        /// <summary>
        /// Creates a fake <see cref="IHostBuilder"/> which is robust enough for common testing scenarios.
        /// </summary>
        /// <param name="configureServices">An optional parameter to allow callers to configure additional services that will participate in dependency injection.</param>
        /// <returns>A fake <see cref="IHostBuilder"/> which is robust enough for common testing scenarios.</returns>
        public static IHostBuilder Create(Action<IServiceCollection> configureServices = null)
        {
            IHostBuilder result = new HostBuilder();
            return result.ConfigureServices(
                sc =>
                {
                    sc.AddSingleton(CreateFakeServiceScopeFactory);
                    configureServices?.Invoke(sc);
                });
        }

        private static IServiceScopeFactory CreateFakeServiceScopeFactory(IServiceProvider serviceProvider)
        {
            IServiceScope CreateFakeServiceScope()
            {
                var mockServiceScope = new Mock<IServiceScope>();
                mockServiceScope.Setup(ss => ss.ServiceProvider).Returns(serviceProvider);
                return mockServiceScope.Object;
            }

            var mockServiceScopeFactory = new Mock<IServiceScopeFactory>();
            mockServiceScopeFactory.Setup(ssf => ssf.CreateScope()).Returns(CreateFakeServiceScope);
            return mockServiceScopeFactory.Object;
        }
    }
}
