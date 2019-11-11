using System;
using System.Collections.Generic;
using System.Linq;

using AutoFixture;

using Moq;

namespace FGS.Tests.Support.AutoFixture.Mocking
{
    /// <summary>
    /// Extends <see cref="IFixture"/> with functionality for registering mocks.
    /// </summary>
    public static class AutoFixtureExtensions
    {
        /// <summary>
        /// Registers a <see cref="Moq.Mock{T}"/> with <paramref name="fixture"/>.
        /// </summary>
        /// <param name="fixture">The <see cref="IFixture"/> into which the mock will be registered.</param>
        /// <typeparam name="T">The type of Mock to register and return.</typeparam>
        /// <returns>The registered <see cref="Moq.Mock{T}"/> so that it configuration calls can be chained to it.</returns>
        public static Mock<T> Mock<T>(this IFixture fixture)
            where T : class
        {
            return fixture.Mock<T>(() => new Mock<T>());
        }

        /// <summary>
        /// Registers a <see cref="Moq.Mock{T}"/> with <paramref name="fixture"/>, specifying the <see cref="MockBehavior"/> of the mock.
        /// </summary>
        /// <param name="fixture">The <see cref="IFixture"/> into which the mock will be registered.</param>
        /// <param name="behavior">Specifies the behavior of the mock.</param>
        /// <typeparam name="T">The type of Mock to register and return.</typeparam>
        /// <returns>The registered <see cref="Moq.Mock{T}"/> so that it configuration calls can be chained to it.</returns>
        public static Mock<T> Mock<T>(this IFixture fixture, MockBehavior behavior)
            where T : class
        {
            return fixture.Mock<T>(() => new Mock<T>(behavior));
        }

        /// <summary>
        /// Registers a <see cref="Moq.Mock{T}"/> with <paramref name="fixture"/>, specifying the arguments that the generated
        /// Mock type should pass to the base constructor.
        /// </summary>
        /// <param name="fixture">The <see cref="IFixture"/> into which the mock will be registered.</param>
        /// <param name="args">The arguments that the generated Mock type will pass to the base constructor.</param>
        /// <typeparam name="T">The type of Mock to register and return.</typeparam>
        /// <returns>The registered <see cref="Moq.Mock{T}"/> so that it configuration calls can be chained to it.</returns>
        public static Mock<T> Mock<T>(this IFixture fixture, params object[] args)
            where T : class
        {
            return fixture.Mock<T>(() => new Mock<T>(MockBehavior.Default, args));
        }

        /// <summary>
        /// Registers a <see cref="Moq.Mock{T}"/> with <paramref name="fixture"/>, specifying the <see cref="MockBehavior"/> of the mock
        /// and the arguments that the generated Mock type should pass to the base constructor.
        /// </summary>
        /// <param name="fixture">The <see cref="IFixture"/> into which the mock will be registered.</param>
        /// <param name="behavior">Specifies the behavior of the mock.</param>
        /// <param name="args">The arguments that the generated Mock type will pass to the base constructor.</param>
        /// <typeparam name="T">The type of Mock to register and return.</typeparam>
        /// <returns>The registered <see cref="Moq.Mock{T}"/> so that it configuration calls can be chained to it.</returns>
        public static Mock<T> Mock<T>(this IFixture fixture, MockBehavior behavior, params object[] args)
            where T : class
        {
            return fixture.Mock<T>(() => new Mock<T>(behavior, args));
        }

        /// <summary>
        /// Registers multiple instances of <see cref="Moq.Mock{T}"/> with <paramref name="fixture"/>.
        /// </summary>
        /// <typeparam name="T">The type of Mock to register and return.</typeparam>
        /// <param name="fixture">The <see cref="IFixture"/> into which the mocks will be registered.</param>
        /// <param name="count">The number of mocks to create and register.</param>
        /// <returns>The registered <see cref="Moq.Mock{T}"/>s so that additional configuration calls can be made to them.</returns>
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
