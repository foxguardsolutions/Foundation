using System;

using Autofac;

using FGS.Autofac.DynamicScoping.Abstractions;

using Microsoft.Extensions.Options;

using Moq;

namespace FGS.Tests.Support.Autofac.Mocking.Options
{
    public static class ContainerBuilderExtensions
    {
        public static void RegisterMockOptions<TOptions>(this ContainerBuilder builder, TOptions optionsValue)
            where TOptions : class, new()
        {
            builder.RegisterMockOptions(() => optionsValue, Scope.Singleton);
        }

        public static void RegisterMockOptions<TOptions>(this ContainerBuilder builder, Func<TOptions> optionsProvider, Scope scope = Scope.Singleton)
            where TOptions : class, new()
        {
            var mockOptions = builder.RegisterMock<IOptions<TOptions>>(scope);
            mockOptions.Setup(o => o.Value).Returns(optionsProvider);

            var mockOptionsFactory = builder.RegisterMock<IOptionsFactory<TOptions>>(scope);
            mockOptionsFactory.Setup(o => o.Create(It.IsAny<string>())).Returns(optionsProvider);

            var mockOptionsSnapshot = builder.RegisterMock<IOptionsSnapshot<TOptions>>(scope);
            mockOptionsSnapshot.Setup(o => o.Value).Returns(optionsProvider);
            mockOptionsSnapshot.Setup(o => o.Get(It.IsAny<string>())).Returns(optionsProvider);
        }

        public static void RegisterNullOptions(this ContainerBuilder builder, Scope scope = Scope.Singleton)
        {
            builder.RegisterUnboundMock(typeof(IOptions<>), scope);
            builder.RegisterUnboundMock(typeof(IOptionsFactory<>), scope);
            builder.RegisterUnboundMock(typeof(IOptionsSnapshot<>), scope);
        }
    }
}
