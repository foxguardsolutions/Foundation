using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Autofac;

using AutoFixture;

using FGS.Autofac.DynamicScoping.Abstractions;
using FGS.FaultHandling.Abstractions;
using FGS.FaultHandling.Abstractions.Retry;
using FGS.Tests.Support.Autofac.Mocking;
using FGS.Tests.Support.Autofac.Mocking.Logging;
using FGS.Tests.Support.Autofac.Mocking.Options;
using FGS.Tests.Support.TestCategories;

using Moq;

using NUnit.Framework;

namespace FGS.Autofac.Interceptors.FaultHandling.Tests
{
    [Unit]
    [TestFixture]
    public class RetryPolicyFactorySociableTests
    {
        private const int RETRY_MAX_ATTEMPTS = 3;

        private Fixture _fixture;
        private Mock<IRetryBackoffCalculator> _mockRetryBackoffCalculator;
        private List<Func<Exception, bool>> _exceptionPredicates;
        private IRetryPolicyFactory _retryPolicyFactory;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();

            var containerBuilder = new ContainerBuilder();
            AugmentRegistrations(containerBuilder);
            var container = containerBuilder.Build();

            _retryPolicyFactory = container.Resolve<IRetryPolicyFactory>();

            _mockRetryBackoffCalculator.Setup(rbc => rbc.CalculateBackoff(It.IsAny<int>())).Returns(TimeSpan.Zero);

            _exceptionPredicates = new List<Func<Exception, bool>> { ex => ex.GetType() == typeof(TestException) };
        }

        private void AugmentRegistrations(ContainerBuilder builder)
        {
            builder.RegisterModule<FaultHandlingModule>();

            builder.RegisterNullLogging();

            var configurationBuilder = _fixture.Build<FaultHandlingConfiguration>().With(c => c.MaxRetries, RETRY_MAX_ATTEMPTS);
            builder.RegisterMockOptions(configurationBuilder.Create);

            _mockRetryBackoffCalculator = builder.RegisterMock<IRetryBackoffCalculator>(Scope.PerDependency);
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

            var retryAttempts = await CallExecuteWithFaultingActionAsync<TestException>(retryPolicy, numberOfAttempts);

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

            Assert.That(() => CallExecuteWithFaultingActionAsync<TestException>(retryPolicy, RETRY_MAX_ATTEMPTS + 1), Throws.InstanceOf<TestException>());
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

        private static async Task<int> CallExecuteWithFaultingActionAsync<TException>(IRetryPolicy policy, int numberOfTimesToThrowException)
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

#pragma warning disable CA1032 // Implement standard exception constructors
#pragma warning disable CA2237 // Mark ISerializable types with serializable
        public class TestException : Exception
#pragma warning restore CA2237 // Mark ISerializable types with serializable
#pragma warning restore CA1032 // Implement standard exception constructors
        {
        }
    }
}
