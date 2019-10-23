using Autofac;

using FGS.Autofac.DynamicScoping;
using FGS.Autofac.DynamicScoping.Abstractions;

using Microsoft.Extensions.Configuration;

namespace FGS.Tests.Support.Autofac.Mocking.Configuration
{
    public static class ContainerBuilderExtensions
    {
        public static void RegisterNullConfiguration(this ContainerBuilder builder, Scope scope = Scope.Singleton)
        {
            builder.Register(ctx => new ConfigurationBuilder().Build()).AsImplementedInterfaces().In(scope);
        }
    }
}
