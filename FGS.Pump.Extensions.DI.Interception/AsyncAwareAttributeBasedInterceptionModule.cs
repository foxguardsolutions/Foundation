using System;
using System.Collections.Generic;

using Autofac;
using Autofac.Core;

using Castle.DynamicProxy;
using FGS.Pump.Extensions.DI;

namespace FGS.Pump.Extensions.DI.Interception
{
    public class AsyncAwareAttributeBasedInterceptionModule<TAttribute, TSyncInterceptor, TAsyncInterceptor>
        : AttributeBasedInterceptionModuleBase<TAttribute>
        where TAttribute : Attribute
        where TSyncInterceptor : IInterceptor
        where TAsyncInterceptor : NonRacingAsyncInterceptor
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<AsyncAwareSwitchingInterceptor<TSyncInterceptor, TAsyncInterceptor, TAttribute>>()
                .WithParameterTypedFrom(ctx => (Func<IInvocation, TAttribute>)InterceptorInstanciationDataFactory)
                .InstancePerDependency();
            builder.RegisterType<TSyncInterceptor>().InstancePerDependency();
            builder.RegisterType<TAsyncInterceptor>().InstancePerDependency();
        }

        protected override IEnumerable<Service> DescribeInterceptorServices(Type originalImplementationType)
        {
            yield return new TypedService(typeof(AsyncAwareSwitchingInterceptor<TSyncInterceptor, TAsyncInterceptor, TAttribute>));
        }
    }
}