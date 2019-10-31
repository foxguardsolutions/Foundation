using Autofac;

using FGS.Autofac.DynamicScoping.Abstractions;

using Microsoft.Extensions.Logging;

namespace FGS.Tests.Support.Autofac.Mocking.Logging
{
    /// <summary>
    /// Extends <see cref="ContainerBuilder"/> with the ability to register null-patterned implementations of <see cref="ILogger"/> and friends.
    /// </summary>
    public static class ContainerBuilderExtensions
    {
        /// <summary>
        /// Registers a null-pattern implementation of <see cref="ILogger"/> and friends with the <paramref name="builder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="ContainerBuilder"/> into which to add the registrations.</param>
        /// <param name="scope">The lifetime management semantics by which the components will be resolved.</param>
        public static void RegisterNullLogging(this ContainerBuilder builder, Scope scope = Scope.Singleton)
        {
            builder.RegisterMock<ILoggerFactory>(scope);
            builder.RegisterMock<ILoggerProvider>(scope);
            builder.RegisterMock<ILogger>(scope);
            builder.RegisterUnboundMock(typeof(ILogger<>), scope);
        }
    }
}
