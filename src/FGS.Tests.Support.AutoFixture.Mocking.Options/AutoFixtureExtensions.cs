using System;

using AutoFixture;

using Microsoft.Extensions.Options;

using Moq;

namespace FGS.Tests.Support.AutoFixture.Mocking.Options
{
    /// <summary>
    /// Extends <see cref="IFixture"/> with the ability to register mocked implementations of <see cref="IOptions{TOptions}"/> and friends.
    /// </summary>
    public static class AutoFixtureExtensions
    {
        /// <summary>
        /// Registers mock Options services (of type <typeparamref name="TOptions"/>) into <paramref name="fixture"/>, all of which
        /// will always return a static anonymous <typeparamref name="TOptions"/> value.
        /// </summary>
        /// <param name="fixture">The <see cref="IFixture"/> into which to add the registrations.</param>
        /// <typeparam name="TOptions">The type of Options value to wire up.</typeparam>
        public static void RegisterMockOptions<TOptions>(this IFixture fixture)
            where TOptions : class, new()
        {
            fixture.RegisterMockOptions(fixture.Create<TOptions>);
        }

        /// <summary>
        /// Registers mock Options services (of type <typeparamref name="TOptions"/>) into <paramref name="fixture"/>, all of which
        /// will always return the value of <paramref name="optionsValue"/>.
        /// </summary>
        /// <param name="fixture">The <see cref="IFixture"/> into which to add the registrations.</param>
        /// <param name="optionsValue">The Options value that will always be returned by the matching Options services.</param>
        /// <typeparam name="TOptions">The type of Options value to wire up.</typeparam>
        public static void RegisterMockOptions<TOptions>(this IFixture fixture, TOptions optionsValue)
            where TOptions : class, new()
        {
            fixture.RegisterMockOptions(() => optionsValue);
        }

        /// <summary>
        /// Registers mock Options services (of type <typeparamref name="TOptions"/>) into <paramref name="fixture"/>, all of which
        /// will always execute <paramref name="optionsProvider"/> and return the result.
        /// </summary>
        /// <typeparam name="TOptions">The type of Options value to wire up.</typeparam>
        /// <param name="fixture">The <see cref="IFixture"/> into which to add the registrations.</param>
        /// <param name="optionsProvider">A factory that creates or retrieves the Options value each time it is requested.</param>
        public static void RegisterMockOptions<TOptions>(this IFixture fixture, Func<TOptions> optionsProvider)
            where TOptions : class, new()
        {
            var mockOptions = fixture.Mock<IOptions<TOptions>>();
            mockOptions.Setup(o => o.Value).Returns(optionsProvider);

            var mockOptionsFactory = fixture.Mock<IOptionsFactory<TOptions>>();
            mockOptionsFactory.Setup(o => o.Create(It.IsAny<string>())).Returns(optionsProvider);

            var mockOptionsSnapshot = fixture.Mock<IOptionsSnapshot<TOptions>>();
            mockOptionsSnapshot.Setup(o => o.Value).Returns(optionsProvider);
            mockOptionsSnapshot.Setup(o => o.Get(It.IsAny<string>())).Returns(optionsProvider);
        }
    }
}
