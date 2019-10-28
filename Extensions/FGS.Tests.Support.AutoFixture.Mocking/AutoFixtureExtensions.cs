using System;
using System.Collections.Generic;
using System.Linq;

using AutoFixture;

using Moq;

namespace FGS.Tests.Support.AutoFixture.Mocking
{
    public static class AutoFixtureExtensions
    {
        public static Mock<T> Mock<T>(this IFixture fixture)
            where T : class
        {
            return fixture.Mock<T>(() => new Mock<T>());
        }

        public static Mock<T> Mock<T>(this IFixture fixture, MockBehavior behavior)
            where T : class
        {
            return fixture.Mock<T>(() => new Mock<T>(behavior));
        }

        public static Mock<T> Mock<T>(this IFixture fixture, params object[] args)
            where T : class
        {
            return fixture.Mock<T>(() => new Mock<T>(MockBehavior.Default, args));
        }

        public static Mock<T> Mock<T>(this IFixture fixture, MockBehavior behavior, params object[] args)
            where T : class
        {
            return fixture.Mock<T>(() => new Mock<T>(behavior, args));
        }

        public static IEnumerable<Mock<T>> MockMany<T>(this IFixture fixture, int count = 3)
            where T : class
        {
            var mocks = Enumerable.Range(0, count).Select(_ => new Mock<T>()).ToList();
            mocks.ForEach(m => fixture.Register(() => m.Object));
            return mocks;
        }

        private static Mock<T> Mock<T>(this IFixture fixture, Func<Mock<T>> mockFactory)
            where T : class
        {
            var mock = mockFactory();
            fixture.Inject(mock);
            fixture.Register(() => fixture.Create<Mock<T>>().Object);
            fixture.Freeze<T>();
            return mock;
        }
    }
}
