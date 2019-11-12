using Autofac;

using FGS.Interception.Abstractions;
using FGS.Interception.DynamicProxy;

namespace FGS.Autofac.Interception.DynamicProxy
{
    /// <summary>
    /// Contains registrations common to all interceptors.
    /// </summary>
    public class InterceptorAdapterModule : Module
    {
        /// <inheritdoc />
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<InvocationAdapter>().AsSelf().As<IInvocation>().IfNotRegistered(typeof(InvocationAdapter)).InstancePerDependency();
            builder.RegisterType<AsyncInvocationAdapter>().AsSelf().As<IAsyncInvocation>().IfNotRegistered(typeof(AsyncInvocationAdapter)).InstancePerDependency();
        }
    }
}
