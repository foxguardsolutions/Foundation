using Autofac;

using FGS.Autofac.DynamicScoping.Abstractions;

using Microsoft.Extensions.Logging;

namespace FGS.Tests.Support.Autofac.Mocking.Logging
{
    public static class ContainerBuilderExtensions
    {
        public static void RegisterNullLogging(this ContainerBuilder builder, Scope scope = Scope.Singleton)
        {
            builder.RegisterMock<ILoggerFactory>(scope);
            builder.RegisterMock<ILoggerProvider>(scope);
            builder.RegisterMock<ILogger>(scope);
            builder.RegisterUnboundMock(typeof(ILogger<>), scope);
        }
    }
}
