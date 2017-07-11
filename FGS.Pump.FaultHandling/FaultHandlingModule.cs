using Autofac;

using FGS.Pump.FaultHandling.Adapters;
using FGS.Pump.FaultHandling.Configuration;
using FGS.Pump.FaultHandling.Retry;

namespace FGS.Pump.FaultHandling
{
    public class FaultHandlingModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<PollyRetryPolicyAdapter>().As<IRetryPolicy>().InstancePerLifetimeScope();

            builder.RegisterType<RetryPolicyFactory>().As<IRetryPolicyFactory>().InstancePerLifetimeScope();

            builder.RegisterType<ExponentialRetryBackoffCalculator>().As<IRetryBackoffCalculator>().SingleInstance();
            builder.RegisterType<AppSettingsFaultHandlingConfiguration>().As<IFaultHandlingConfiguration>().SingleInstance();
        }
    }
}