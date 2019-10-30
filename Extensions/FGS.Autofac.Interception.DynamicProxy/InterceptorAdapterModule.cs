using Autofac;

using FGS.Interception.Abstractions;
using FGS.Interception.DynamicProxy;

namespace FGS.Autofac.Interception.DynamicProxy
{
    public class InterceptorAdapterModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<InvocationAdapter>().AsSelf().As<IInvocation>().IfNotRegistered(typeof(InvocationAdapter)).InstancePerDependency();
            builder.RegisterType<AsyncInvocationAdapter>().AsSelf().As<IAsyncInvocation>().IfNotRegistered(typeof(AsyncInvocationAdapter)).InstancePerDependency();
        }
    }
}
