using Autofac;

using FGS.Autofac.Interception.DynamicProxy;
using FGS.Autofac.Registration.Extensions;
using FGS.Interception.Annotations.Time;
using FGS.Interceptors.Time;
using FGS.Primitives.Time;
using FGS.Primitives.Time.Abstractions;

namespace FGS.Autofac.Interceptors.Time
{
    /// <summary>
    /// Registers the <see cref="FreezeTimeInterceptor"/> to intercept virtual members of types annotated with the <see cref="FreezeTimeAttribute"/>.
    /// This causes resolved <see cref="IClock"/> instances to reflect a frozen instant in time for the duration of the intercepted call.
    /// </summary>
    public sealed class FreezeTimeModule : Module
    {
        /// <inheritdoc />
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<SystemClock>()
                .Named<IClock>(nameof(SystemClock))
                .SingleInstance();

            builder.RegisterType<FreezableClock>()
                .As<IClock>()
                .As<IFreezableClock>()
                .WithParameterTypedFrom(ctx => ctx.ResolveNamed<IClock>(nameof(SystemClock)))
                .InstancePerLifetimeScope();

            builder.RegisterModule<InterceptorAdapterModule>();

            RegisterInterceptor(builder);
        }

        private static void RegisterInterceptor(ContainerBuilder builder)
        {
            builder.RegisterModule<AttributeBasedInterceptionModule<FreezeTimeAttribute, FreezeTimeInterceptor>>();
        }
    }
}
