using Autofac;

using FGS.Autofac.Interception.DynamicProxy;
using FGS.Pump.FaultHandling.Adapters;
using FGS.Pump.FaultHandling.Configuration;
using FGS.Pump.FaultHandling.Interception;
using FGS.Pump.FaultHandling.Retry;
using FGS.Pump.Settings;

namespace FGS.Pump.FaultHandling
{
    public class FaultHandlingModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<RetryPolicyCoordinator>().As<IRetryPolicyCoordinator>().InstancePerLifetimeScope();

            builder.RegisterType<PollyRetryPolicyAdapter>().As<IRetryPolicy>().InstancePerLifetimeScope();
            builder.RegisterType<NoOpRetryPolicy>().AsSelf().SingleInstance();

            builder.RegisterType<RetryPolicyFactory>().As<IRetryPolicyFactory>().InstancePerLifetimeScope();

            builder.RegisterType<ExponentialRetryBackoffCalculator>().As<IRetryBackoffCalculator>().SingleInstance();
            builder.RegisterType<FaultHandlingConfiguration>().As<IFaultHandlingConfiguration>().WithConfigurationFromSection("FaultHandling").SingleInstance();

            RegisterInterceptor(builder);
        }

        private static void RegisterInterceptor(ContainerBuilder builder)
        {
            builder.RegisterModule<AttributeBasedInterceptionModule<RetryOnFaultAttribute, RetryInterceptor>>();
        }
    }
}
