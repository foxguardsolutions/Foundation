using System;

using AutoFixture;

using Microsoft.Extensions.Options;

using Moq;

namespace FGS.Tests.Support.AutoFixture.Mocking.Options
{
    public static class AutoFixtureExtensions
    {
        public static void RegisterMockOptions<TOptions>(this IFixture fixture)
            where TOptions : class, new()
        {
            fixture.RegisterMockOptions(fixture.Create<TOptions>);
        }

        public static void RegisterMockOptions<TOptions>(this IFixture fixture, TOptions optionsValue)
            where TOptions : class, new()
        {
            fixture.RegisterMockOptions(() => optionsValue);
        }

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
