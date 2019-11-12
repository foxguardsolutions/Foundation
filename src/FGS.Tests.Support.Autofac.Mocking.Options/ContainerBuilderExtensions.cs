using System;

using Autofac;

using FGS.Autofac.DynamicScoping.Abstractions;

using Microsoft.Extensions.Options;

using Moq;

namespace FGS.Tests.Support.Autofac.Mocking.Options
{
    /// <summary>
    /// Extends <see cref="ContainerBuilder"/> with the ability to register mock or null-patterned implementations of <see cref="IOptions{TOptions}"/> and friends.
    /// </summary>
    public static class ContainerBuilderExtensions
    {
        /// <summary>
        /// Registers mock Options services (of type <typeparamref name="TOptions"/>) into <paramref name="builder"/>, all of which
        /// will always return the <paramref name="optionsValue"/> value.
        /// </summary>
        /// <param name="builder">The <see cref="ContainerBuilder"/> into which to add the registrations.</param>
        /// <param name="optionsValue">The Options value that will always be returned by the matching Options services.</param>
        /// <typeparam name="TOptions">The type of Options value to wire up.</typeparam>
        public static void RegisterMockOptions<TOptions>(this ContainerBuilder builder, TOptions optionsValue)
            where TOptions : class, new()
        {
            builder.RegisterMockOptions(() => optionsValue, Scope.Singleton);
        }

        /// <summary>
        /// Registers mock Options services (of type <typeparamref name="TOptions"/>) into <paramref name="builder"/>, all of which
        /// will always execute <paramref name="optionsProvider"/> and return the result.
        /// </summary>
        /// <typeparam name="TOptions">The type of Options value to wire up.</typeparam>
        /// <param name="builder">The <see cref="ContainerBuilder"/> into which to add the registrations.</param>
        /// <param name="optionsProvider">A factory that creates or retrieves the Options value each time it is requested.</param>
        /// <param name="scope">The lifetime management semantics by which the components will be resolved.</param>
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

        /// <summary>
        /// Registers unbound generic Options services into <paramref name="builder"/>, all of which
        /// will always return <see langword="null"/>.
        /// </summary>
        /// <param name="builder">The <see cref="ContainerBuilder"/> into which to add the registrations.</param>
        /// <param name="scope">The lifetime management semantics by which the components will be resolved.</param>
        public static void RegisterNullOptions(this ContainerBuilder builder, Scope scope = Scope.Singleton)
        {
            builder.RegisterUnboundMock(typeof(IOptions<>), scope);
            builder.RegisterUnboundMock(typeof(IOptionsFactory<>), scope);
            builder.RegisterUnboundMock(typeof(IOptionsSnapshot<>), scope);
        }
    }
}
