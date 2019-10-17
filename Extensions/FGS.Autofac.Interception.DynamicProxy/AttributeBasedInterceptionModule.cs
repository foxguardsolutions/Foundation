using System;
using System.Collections.Generic;
using System.Linq;

using Autofac;
using Autofac.Core;

using Castle.DynamicProxy;

using FGS.Interception.DynamicProxy;

using IInterceptor = FGS.Interception.Abstractions.IInterceptor;

namespace FGS.Autofac.Interception.DynamicProxy
{
    public sealed class AttributeBasedInterceptionModule<TAttribute, TInterceptor> : CustomInterceptionModuleBase
        where TAttribute : Attribute
        where TInterceptor : IInterceptor
    {
        private static readonly AttributeProxyGenerationHook<TAttribute> ProxyGenerationHook = new AttributeProxyGenerationHook<TAttribute>();

        protected override IProxyGenerationHook CreateProxyGenerationHook(Type originalImplementationType) => ProxyGenerationHook;

        protected override bool ShouldInterceptType(Type originalImplementationType)
            => originalImplementationType.GetMethods().Any(m => ProxyGenerationHook.ShouldInterceptMethod(originalImplementationType, m));

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register<Func<Castle.DynamicProxy.IInvocation, TAttribute>>(ctx => InterceptorInstantiationDataFactory).InstancePerDependency();
            builder.RegisterType<TInterceptor>().InstancePerDependency();
            builder.RegisterType<InterceptorAdapter<TInterceptor, TAttribute>>().InstancePerDependency();
        }

        protected override IEnumerable<Service> DescribeInterceptorServices(Type originalImplementationType)
        {
            yield return new TypedService(typeof(InterceptorAdapter<TInterceptor, TAttribute>));
        }

        private static TAttribute InterceptorInstantiationDataFactory(Castle.DynamicProxy.IInvocation invocation)
        {
            return (TAttribute)invocation.MethodInvocationTarget.GetCustomAttributes(typeof(TAttribute), inherit: true).SingleOrDefault()
                   ?? (TAttribute)invocation.TargetType.GetCustomAttributes(typeof(TAttribute), inherit: true).SingleOrDefault();
        }
    }
}
