using Autofac;

using FGS.Autofac.Interception.DynamicProxy;
using FGS.Autofac.Options;
using FGS.FaultHandling.Abstractions;
using FGS.FaultHandling.Abstractions.Retry;
using FGS.FaultHandling.Polly.Retry;
using FGS.FaultHandling.Primitives.Retry;
using FGS.Interception.Annotations.FaultHandling;
using FGS.Interceptors.FaultHandling.Retry;

using Microsoft.Extensions.Configuration;

namespace FGS.Autofac.Interceptors.FaultHandling
{
    /// <summary>
    /// <para>Registers interception such that types annotated with <see cref="RetryOnFaultAttribute"/> will have their virtual members intercepted by
    /// <see cref="RetryInterceptor"/>, which will retry failed operations with an exponential back-off calculated by <see cref="ExponentialRetryBackoffCalculator"/>.</para>
    /// <para>Assumes that zero or more instances of <see cref="IExceptionRetryPredicate"/> are registered, which help shape the retry behavior.</para>
    /// <para>Assumes an <see cref="IConfiguration"/> is resolvable, with a section named <value>FaultHandling</value> that is mappable to <see cref="FaultHandlingConfiguration"/>.</para>
    /// </summary>
    public sealed class FaultHandlingModule : Module
    {
        /// <inheritdoc />
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<RetryPolicyFactory>().As<IRetryPolicyFactory>().InstancePerLifetimeScope();
            builder.RegisterType<RetryPolicyCoordinator>().As<IRetryPolicyCoordinator>().InstancePerLifetimeScope();

            builder.RegisterType<PollyRetryPolicyAdapter>().As<IRetryPolicy>().InstancePerLifetimeScope();
            builder.RegisterType<NoOpRetryPolicy>().AsSelf().SingleInstance();

            builder.Configure<FaultHandlingConfiguration>(ctx => ctx.Resolve<IConfiguration>().GetSection("FaultHandling"));

            builder.RegisterType<ExponentialRetryBackoffCalculator>().As<IRetryBackoffCalculator>().SingleInstance();

            builder.RegisterModule<InterceptorAdapterModule>();

            RegisterInterceptor(builder);
        }

        private static void RegisterInterceptor(ContainerBuilder builder)
        {
            builder.RegisterModule<AttributeBasedInterceptionModule<RetryOnFaultAttribute, RetryInterceptor>>();
        }
    }
}
