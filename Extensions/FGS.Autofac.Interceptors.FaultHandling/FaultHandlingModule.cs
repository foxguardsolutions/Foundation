using Autofac;

using FGS.Autofac.Interception.DynamicProxy;
using FGS.Autofac.Options;
using FGS.FaultHandling.Abstractions;
using FGS.FaultHandling.Abstractions.Retry;
using FGS.FaultHandling.Polly.Retry;
using FGS.FaultHandling.Primitives.Retry;
using FGS.Interception.Annotations.FaultHandling;
using FGS.Interception.FaultHandling.Retry;

using Microsoft.Extensions.Configuration;

namespace FGS.Autofac.Interceptors.FaultHandling
{
    public class FaultHandlingModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<RetryPolicyFactory>().As<IRetryPolicyFactory>().InstancePerLifetimeScope();
            builder.RegisterType<RetryPolicyCoordinator>().As<IRetryPolicyCoordinator>().InstancePerLifetimeScope();

            builder.RegisterType<PollyRetryPolicyAdapter>().As<IRetryPolicy>().InstancePerLifetimeScope();
            builder.RegisterType<NoOpRetryPolicy>().AsSelf().SingleInstance();

            builder.Configure<FaultHandlingConfiguration>(ctx => ctx.Resolve<IConfiguration>().GetSection("FaultHandling"));

            builder.RegisterType<ExponentialRetryBackoffCalculator>().As<IRetryBackoffCalculator>().SingleInstance();

            RegisterInterceptor(builder);
        }

        private static void RegisterInterceptor(ContainerBuilder builder)
        {
            builder.RegisterModule<AttributeBasedInterceptionModule<RetryOnFaultAttribute, RetryInterceptor>>();
        }
    }
}
