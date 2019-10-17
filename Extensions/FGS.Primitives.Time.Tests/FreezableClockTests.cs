using System;

using FGS.Primitives.Time.Abstractions;

using Moq;

using NUnit.Framework;

namespace FGS.Primitives.Time.Tests
{
    [Category("Unit")]
    [TestFixture]
    public class FreezableClockTests
    {
        private Mock<IClock> _mockClock;
        private FreezableClock _subject;
        private int _numberOfRequestsForTime;

        [SetUp]
        public void Setup()
        {
            _mockClock = new Mock<IClock>();

            _subject = new FreezableClock(_mockClock.Object);

            _numberOfRequestsForTime = new Random().Next(1, 10);
        }

        [Test]
        public void Now_WhenNotFrozen_AlwaysReturnsCurrentTime()
        {
            for (var i = 0; i < _numberOfRequestsForTime; i++)
            {
                var time = _subject.Now;
            }

            _mockClock.VerifyGet(c => c.Now, Times.Exactly(_numberOfRequestsForTime));
        }

        [Test]
        public void UtcNow_WhenNotFrozen_AlwaysReturnsCurrentUtcTime()
        {
            for (var i = 0; i < _numberOfRequestsForTime; i++)
            {
                var time = _subject.UtcNow;
            }

            _mockClock.VerifyGet(c => c.UtcNow, Times.Exactly(_numberOfRequestsForTime));
        }

        [Test]
        public void Now_WhenFrozen_FreezesAtTheFirstRequest()
        {
            var numberOfRequests = new Random().Next(1, 10);
            _subject.FreezeTime();

            for (var i = 0; i < numberOfRequests; i++)
            {
                var time = _subject.Now;
            }

            _mockClock.VerifyGet(c => c.Now, Times.Once);
        }

        [Test]
        public void UtcNow_WhenFrozen_FreezesAtTheFirstRequest()
        {
            _subject.FreezeTime();

            for (var i = 0; i < _numberOfRequestsForTime; i++)
            {
                var time = _subject.UtcNow;
            }

            _mockClock.VerifyGet(c => c.UtcNow, Times.Once);
        }

        [Test]
        public void Now_WhenUnfrozen_AlwaysReturnsCurrentTime()
        {
            _subject.FreezeTime();
            _subject.UnfreezeTime();

            for (var i = 0; i < _numberOfRequestsForTime; i++)
            {
                var time = _subject.Now;
            }

            _mockClock.VerifyGet(c => c.Now, Times.Exactly(_numberOfRequestsForTime));
        }

        [Test]
        public void UtcNow_WhenUnfrozen_AlwaysReturnsCurrentUtcTime()
        {
            _subject.FreezeTime();
            _subject.UnfreezeTime();

            for (var i = 0; i < _numberOfRequestsForTime; i++)
            {
                var time = _subject.UtcNow;
            }

            _mockClock.VerifyGet(c => c.UtcNow, Times.Exactly(_numberOfRequestsForTime));
        }
    }
}
