using System;
using System.Threading.Tasks;
using FGS.Tests.Support.Extensions.Hosting.Middleware.Mocking;
using FGS.Tests.Support.TestCategories;
using FGS.Tests.Support;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using System.Threading;

namespace FGS.Extensions.Hosting.Middleware.DelayedStart.Tests
{
    [Unit]
    [TestFixture]
    public class DelayedStartHostBuilderExtensionsTests
    {
        private IHostBuilder _hostBuilder;
        private AutoFixture.Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            HostSpy.Reset();
            _fixture = new AutoFixture.Fixture();
            _hostBuilder = CreateHostBuilder();
        }

        [Test]
        public async Task WithDelayedStart_GivenDelay_Delays()
        {
            var delay = Given_DelayedStart();

            var executionTime = await BuildAndRunForDurationAndMeasureExecutionTimeAsync(delay).ConfigureAwait(false);

            executionTime.Should().BeGreaterOrEqualTo(delay);
        }

        [Test]
        public async Task WithDelayedStart_GivenDelay_StillCallsHostStart()
        {
            var delay = Given_DelayedStart();

            await BuildAndRunForDurationAsync(delay).ConfigureAwait(false);

            Verify_HostStart_Called();
        }

        [Test]
        public async Task WithDelayedStart_GivenDelay_StillCallsHostStop()
        {
            var delay = Given_DelayedStart();

            await BuildAndRunForDurationAsync(delay).ConfigureAwait(false);

            Verify_HostStop_Called();
        }

        private static IHostBuilder CreateHostBuilder() =>
            FakedServiceScopeHostBuilderFactory.Create(sc => sc.AddSingleton<IHost, HostSpy>());

        private void Verify_HostStart_Called() =>
            HostSpy.StartAsyncCallCount.Should().Be(1);

        private void Verify_HostStop_Called() =>
            HostSpy.StopAsyncCallCount.Should().Be(1);

        private TimeSpan Given_DelayedStart()
        {
            var delay = TimeSpan.FromSeconds(_fixture.CreateTinyPositiveRandomNumber());

            _hostBuilder.ConfigureServices(sc => sc.AddScoped<HostCallingHostingMiddleware>());
            _hostBuilder = _hostBuilder.WithDelayedStart(delay);

            return delay;
        }

        private async Task<TimeSpan> BuildAndRunForDurationAndMeasureExecutionTimeAsync(TimeSpan minimumDuration)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                await BuildAndRunForDurationAsync(minimumDuration).ConfigureAwait(false);
            }
            finally
            {
                stopwatch.Stop();
            }

            return stopwatch.Elapsed;
        }

        /// <remarks>
        /// This fires a task and then asynchronously cancels it because we are working around the fact that
        /// the default implementation of <see cref="HostingAbstractionsHostExtensions.RunAsync(IHost, CancellationToken)"/> will endlessly
        /// wait until cancelled.
        /// </remarks>
        private async Task BuildAndRunForDurationAsync(TimeSpan minimumDuration)
        {
            var maximumDuration = minimumDuration + TimeSpan.FromSeconds(1);

            using var cancellationTokenSource = new CancellationTokenSource();
            
            var executionTask = _hostBuilder.Build().RunAsync(cancellationTokenSource.Token);

            await Task.Delay(maximumDuration).ConfigureAwait(false);

            cancellationTokenSource.Cancel();

            try
            {
                await executionTask.ConfigureAwait(false);
            }
            catch (TaskCanceledException)
            {
            }
        }
    }
}
