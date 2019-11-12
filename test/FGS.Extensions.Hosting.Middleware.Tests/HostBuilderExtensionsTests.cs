using System;
using System.Threading;
using System.Threading.Tasks;
using FGS.Extensions.Hosting.Middleware.Tests.Mocks;
using FGS.Tests.Support.TestCategories;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using NUnit.Framework;

namespace FGS.Extensions.Hosting.Middleware.Tests
{
    [Unit]
    [TestFixture]
    public class HostBuilderExtensionsTests
    {
        private IHostBuilder _hostBuilder;

        [SetUp]
        public void SetUp()
        {
            HostSpy.Reset();
            _hostBuilder = CreateHostBuilder();
        }

        [Test]
        public async Task UseHostingMiddleware_GivenHostCallingHostingMiddleware_BuildAndRunAsync_OnStart_CallsHostStart()
        {
            Given_HostCallingHostingMiddleware();

            await BuildAndRunAsync();

            Verify_HostStart_Called();
        }

        [Test]
        public async Task UseHostingMiddleware_GivenHostCallingHostingMiddleware_BuildAndRunAsync_OnStop_CallsHostStop()
        {
            Given_HostCallingHostingMiddleware();

            await BuildAndRunAsync();

            Verify_HostStop_Called();
        }

        [Test]
        public async Task UseHostingMiddleware_GivenHostSkippingHostingMiddleware_BuildAndRunAsync_OnStart_DoesNotCallHostStart()
        {
            Given_HostSkippingHostingMiddleware();

            await BuildAndRunAsync();

            Verify_HostStart_NotCalled();
        }

        [Test]
        public async Task UseHostingMiddleware_GivenHostSkippingHostingMiddleware_BuildAndRunAsync_OnStop_DoesNotCallHostStop()
        {
            Given_HostSkippingHostingMiddleware();

            await BuildAndRunAsync();

            Verify_HostStop_NotCalled();
        }

        private static IHostBuilder CreateHostBuilder()
        {
            IHostBuilder result = new HostBuilder();
            return result.ConfigureServices(
                sc =>
                {
                    sc.AddSingleton<IHost, HostSpy>();
                    sc.AddSingleton(CreateFakeServiceScopeFactory);
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

        private void Verify_HostStart_Called()
        {
            HostSpy.StartAsyncCallCount.Should().Be(1);
        }

        private void Verify_HostStop_Called()
        {
            HostSpy.StopAsyncCallCount.Should().Be(1);
        }

        private void Verify_HostStart_NotCalled()
        {
            HostSpy.StartAsyncCallCount.Should().Be(0);
        }

        private void Verify_HostStop_NotCalled()
        {
            HostSpy.StopAsyncCallCount.Should().Be(0);
        }

        private void Given_HostCallingHostingMiddleware()
        {
            _hostBuilder.ConfigureServices(sc => sc.AddScoped<HostCallingHostingMiddleware>());
            _hostBuilder = _hostBuilder.UseHostingMiddleware(sp => sp.GetRequiredService<HostCallingHostingMiddleware>());
        }

        private void Given_HostSkippingHostingMiddleware()
        {
            _hostBuilder.ConfigureServices(sc => sc.AddScoped<HostSkippingHostingMiddleware>());
            _hostBuilder = _hostBuilder.UseHostingMiddleware(sp => sp.GetRequiredService<HostSkippingHostingMiddleware>());
        }

        private Task BuildAndRunAsync()
        {
            var immediatelyCancelToken = new CancellationToken(true);
            return _hostBuilder.Build().RunAsync(immediatelyCancelToken);
        }
    }
}
