using Autofac;

using FGS.Pump.Extensions.DI.Interception;
using FGS.Pump.FaultHandling.Adapters;
using FGS.Pump.FaultHandling.Configuration;
using FGS.Pump.FaultHandling.Interception;
using FGS.Pump.FaultHandling.Retry;

namespace FGS.Pump.FaultHandling
{
    public class FaultHandlingModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<RetryPolicyCoordinator>().As<IRetryPolicyCoordinator>().InstancePerLifetimeScope();

            builder.RegisterType<PollyRetryPolicyAdapter>().As<IRetryPolicy>().InstancePerLifetimeScope();
            builder.RegisterType<NoOpRetryPolicy>().AsSelf().SingleInstance();

            builder.RegisterType<RetryPolicyFactory>().As<IRetryPolicyFactory>().InstancePerLifetimeScope();

            builder.RegisterType<ExponentialRetryBackoffCalculator>().As<IRetryBackoffCalculator>().SingleInstance();
            builder.RegisterType<AppSettingsFaultHandlingConfiguration>().As<IFaultHandlingConfiguration>().SingleInstance();

            RegisterInterceptor(builder);
        }

        private static void RegisterInterceptor(ContainerBuilder builder)
        {
            builder.RegisterModule<AttributeBasedInterceptionModule<RetryOnFaultAttribute, RetryInterceptor>>();
        }
    }
}
