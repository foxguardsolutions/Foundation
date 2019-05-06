using Autofac;

namespace FGS.Pump.Extensions.DI.Interception
{
    public class InterceptorAdapterModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<InvocationAdapter>().As<IInvocation>().InstancePerDependency();
            builder.RegisterType<AsyncInvocationAdapter>().As<IAsyncInvocation>().InstancePerDependency();
        }
    }
}
