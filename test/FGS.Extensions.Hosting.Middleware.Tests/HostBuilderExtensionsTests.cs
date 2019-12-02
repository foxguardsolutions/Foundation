using System.Threading;
using System.Threading.Tasks;
using FGS.Tests.Support.Extensions.Hosting.Middleware.Mocking;
using FGS.Tests.Support.TestCategories;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
            return FakedServiceScopeHostBuilderFactory.Create(sc =>
            {
                sc.AddSingleton<IHost, HostSpy>();
            });
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
