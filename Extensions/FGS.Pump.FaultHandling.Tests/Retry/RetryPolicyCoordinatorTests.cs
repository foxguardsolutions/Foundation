using System;
using System.Collections.Generic;
using System.Linq;

using AutoFixture;

using FGS.Pump.FaultHandling.Adapters;
using FGS.Pump.FaultHandling.Retry;
using FGS.Pump.Tests.Support;
using FGS.Tests.Support;
using FGS.Tests.Support.TestCategories;

using Moq;

using NUnit.Framework;

namespace FGS.Pump.FaultHandling.Tests.Retry
{
    [Unit]
    [TestFixture]
    public class RetryPolicyCoordinatorTests
    {
        private Fixture _fixture;

        private Mock<IRetryPolicyFactory> _mockRetryPolicyFactory;
        private Action _noOpPolicyFactoryHook;

        private RetryPolicyCoordinator _subject;

        [SetUp]
        public void Setup()
        {
            _fixture = AutoFixtureFactory.Create();

            _mockRetryPolicyFactory = _fixture.Mock<IRetryPolicyFactory>();
            _noOpPolicyFactoryHook = () => { };

            _fixture.Register(Enumerable.Empty<IExceptionRetryPredicate>);
            _fixture.Register<Func<NoOpRetryPolicy>>(() => () =>
            {
                _noOpPolicyFactoryHook();
                return new NoOpRetryPolicy();
            });

            _subject = _fixture.Create<RetryPolicyCoordinator>();
        }

        [Test]
        public void RequestPolicy_OnTheFirstRequestForARetryPolicy_GetsAPolicyFromTheRetryPolicyFactory()
        {
            _subject.RequestPolicy();

            _mockRetryPolicyFactory.Verify(rpf => rpf.Create(It.IsAny<IEnumerable<Func<Exception, bool>>>()), Times.Once);
        }

        [TestCaseSource(nameof(RandomNumberOfPolicyRequests))]
        public void RequestPolicy_OnSusequentRequestsForARetryPolicy_GetsANoOpRetryPolicy(int numberOfPolicyRequests)
        {
            var noOpCount = 0;
            _noOpPolicyFactoryHook = () => { noOpCount++; };

            for (var i = 0; i < numberOfPolicyRequests; i++)
            {
                _subject.RequestPolicy();
            }

            Assert.That(noOpCount, Is.EqualTo(numberOfPolicyRequests - 1));
        }

        private static IEnumerable<int> RandomNumberOfPolicyRequests()
        {
            var random = new Random();

            return Enumerable.Range(0, 3).Select(n => random.Next(1, 10));
        }
    }
}
