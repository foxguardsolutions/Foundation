using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Autofac;

using FGS.Pump.Extensions.DI;
using FGS.Pump.FaultHandling.Configuration;
using FGS.Pump.FaultHandling.Retry;
using FGS.Pump.Logging;
using FGS.Pump.Tests.Support;
using FGS.Pump.Tests.Support.Extensions;
using FGS.Pump.Tests.Support.TestCategories;

using Moq;

using NUnit.Framework;

namespace FGS.Pump.FaultHandling.Tests.Retry
{
    [Unit]
    [TestFixture]
    public class RetryPolicyTests : SociableTests
    {
        private const int RETRY_MAX_ATTEMPTS = 3;

        private Mock<IFaultHandlingConfiguration> _mockConfiguration;
        private Mock<IRetryBackoffCalculator> _mockRetryBackoffCalculator;
        private List<Func<Exception, bool>> _exceptionPredicates;
        private IRetryPolicyFactory _retryPolicyFactory;

        [SetUp]
        public new void Setup()
        {
            _retryPolicyFactory = Container.Resolve<IRetryPolicyFactory>();

            _mockConfiguration.SetupGet(c => c.MaxRetries).Returns(RETRY_MAX_ATTEMPTS);
            _mockRetryBackoffCalculator.Setup(rbc => rbc.CalculateBackoff(It.IsAny<int>())).Returns(TimeSpan.Zero);

            _exceptionPredicates = new List<Func<Exception, bool>> { ex => ex.GetType() == typeof(TestException) };
        }

        protected override void AugmentRegistrations(ContainerBuilder builder, Scope httpScope)
        {
            builder.RegisterModule<FaultHandlingModule>();

            _mockConfiguration = Fixture.Mock<IFaultHandlingConfiguration>();
            builder.Register(context => _mockConfiguration.Object).As<IFaultHandlingConfiguration>().SingleInstance();

            builder.Register(context => Fixture.Mock<IStructuralLogger>().Object).As<IStructuralLogger>().SingleInstance();

            _mockRetryBackoffCalculator = Fixture.Mock<IRetryBackoffCalculator>();
            builder.Register(context => _mockRetryBackoffCalculator.Object).As<IRetryBackoffCalculator>().InstancePerDependency();
        }

        [TestCaseSource(nameof(NumberOfAttemptsLessThanMaxRetries))]
        public void Execute_WhenTheActionThrowsAHandledExceptionFewerThanTimesMaxRetries_RetriesTheActionUpToMaxRetriesTimes_AndDoesNotThrowTheException(int numberOfAttempts)
        {
            var retryPolicy = _retryPolicyFactory.Create(_exceptionPredicates);

            var retryAttempts = CallExecuteWithFaultingAction<TestException>(retryPolicy, numberOfAttempts);

            Assert.That(retryAttempts, Is.LessThanOrEqualTo(RETRY_MAX_ATTEMPTS));
        }

        [TestCaseSource(nameof(NumberOfAttemptsLessThanMaxRetries))]
        public async Task ExecuteAsync_WhenTheActionThrowsAHandledExceptionFewerThanTimesMaxRetries_RetriesTheActionUpToMaxRetriesTimes_AndDoesNotThrowTheException(int numberOfAttempts)
        {
            var retryPolicy = _retryPolicyFactory.Create(_exceptionPredicates);

            var retryAttempts = await CallExcecuteWithFaultingActionAsync<TestException>(retryPolicy, numberOfAttempts);

            Assert.That(retryAttempts, Is.LessThanOrEqualTo(RETRY_MAX_ATTEMPTS));
        }

        [Test]
        public void Execute_WhenTheActionThrowsAHandledExceptionMoreThanTimesMaxRetries_ThrowsTheException()
        {
            var retryPolicy = _retryPolicyFactory.Create(_exceptionPredicates);

            Assert.That(() => CallExecuteWithFaultingAction<TestException>(retryPolicy, RETRY_MAX_ATTEMPTS + 1), Throws.InstanceOf<TestException>());
        }

        [Test]
        public void ExecuteAsync_WhenTheActionThrowsAHandledExceptionMoreThanTimesMaxRetries_ThrowsTheException()
        {
            var retryPolicy = _retryPolicyFactory.Create(_exceptionPredicates);

            Assert.That(() => CallExcecuteWithFaultingActionAsync<TestException>(retryPolicy, RETRY_MAX_ATTEMPTS + 1), Throws.InstanceOf<TestException>());
        }

        private static int CallExecuteWithFaultingAction<TException>(IRetryPolicy policy, int numberOfTimesToThrowException)
            where TException : Exception, new()
        {
            var attempts = 0;

            policy.Execute(() =>
            {
                if (attempts >= numberOfTimesToThrowException) return;

                attempts++;

                throw new TException();
            });

            return attempts;
        }

        private static async Task<int> CallExcecuteWithFaultingActionAsync<TException>(IRetryPolicy policy, int numberOfTimesToThrowException)
            where TException : Exception, new()
        {
            var attempts = 0;

            await policy.ExecuteAsync(() =>
            {
                if (attempts >= numberOfTimesToThrowException) return Task.FromResult(attempts);

                attempts++;

                return Task.FromException(new TException());
            });

            return attempts;
        }

        private static IEnumerable<int> NumberOfAttemptsLessThanMaxRetries() => Enumerable.Range(1, RETRY_MAX_ATTEMPTS - 1);

        [Serializable]
        private class TestException : Exception
        {
        }
    }
}