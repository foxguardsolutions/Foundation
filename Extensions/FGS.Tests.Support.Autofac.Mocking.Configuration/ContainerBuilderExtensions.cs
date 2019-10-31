using Autofac;

using FGS.Autofac.DynamicScoping;
using FGS.Autofac.DynamicScoping.Abstractions;

using Microsoft.Extensions.Configuration;

namespace FGS.Tests.Support.Autofac.Mocking.Configuration
{
    /// <summary>
    /// Extends <see cref="ContainerBuilder"/> with the ability to register an empty implementation of <see cref="IConfiguration"/> and friends.
    /// </summary>
    public static class ContainerBuilderExtensions
    {
        /// <summary>
        /// Registers an empty implementation of <see cref="IConfiguration"/> and friends with the <paramref name="builder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="ContainerBuilder"/> into which to add the registration.</param>
        /// <param name="scope">The lifetime management semantics by which the components will be resolved.</param>
        public static void RegisterNullConfiguration(this ContainerBuilder builder, Scope scope = Scope.Singleton)
        {
            builder.Register(ctx => new ConfigurationBuilder().Build()).AsImplementedInterfaces().In(scope);
        }
    }
}
