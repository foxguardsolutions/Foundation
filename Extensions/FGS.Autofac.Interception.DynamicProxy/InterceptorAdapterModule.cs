using Autofac;

using FGS.Interception.Abstractions;
using FGS.Interception.DynamicProxy;

namespace FGS.Autofac.Interception.DynamicProxy
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
