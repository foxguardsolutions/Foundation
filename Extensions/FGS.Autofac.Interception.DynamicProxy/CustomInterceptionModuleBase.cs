using System;
using System.Collections.Generic;
using System.Linq;

using Autofac;
using Autofac.Core;
using Autofac.Core.Activators.Reflection;
using Autofac.Core.Registration;

using Castle.DynamicProxy;

namespace FGS.Autofac.Interception.DynamicProxy
{
    public abstract class CustomInterceptionModuleBase : Module
    {
        private const string ChildRegistrationPropertyName = "FGS.Pump.Extensions.DI" + nameof(CustomInterceptionModuleBase) + "." + nameof(ChildRegistrationPropertyName);
        private const string InterceptorServicesPropertyName = "FGS.Pump.Extensions.DI" + nameof(CustomInterceptionModuleBase) + "." + nameof(InterceptorServicesPropertyName);
        internal static readonly ProxyGenerator ProxyGenerator = new ProxyGenerator();

        protected override void AttachToComponentRegistration(IComponentRegistry componentRegistry, IComponentRegistration registration)
        {
            if (IsChildRegistration(registration) || !HasEligibleActivator(registration))
                return;

            var childRegistration = GetChildRegistration(registration);
            if (childRegistration == null)
            {
                childRegistration = CreateClassInterceptorRegistration(registration);
                SetChildRegistration(registration, childRegistration);
                componentRegistry.Register(childRegistration);
            }

            var existingInterceptorServices = GetInterceptorServices(childRegistration);
            var newInterceptorServices = DescribeInterceptorServices((registration.Activator as ReflectionActivator).LimitType);
            SetInterceptorServices(childRegistration, existingInterceptorServices.Concat(newInterceptorServices));
        }

        protected abstract bool ShouldInterceptType(Type originalImplementationType);

        protected abstract IProxyGenerationHook CreateProxyGenerationHook(Type originalImplementationType);

        protected abstract IEnumerable<Service> DescribeInterceptorServices(Type originalImplementationType);

        /// <remarks>Ported from <see cref="Autofac.Extras.DynamicProxy2.RegistrationExtensions.EnableClassInterceptors{TLimit, TConcreteReflectionActivatorData, TRegistrationStyle}(IRegistrationBuilder{TLimit,TConcreteReflectionActivatorData,TRegistrationStyle}, ProxyGenerationOptions, Type[])"/> found here: https://github.com/autofac/Autofac.Extras.DynamicProxy/blob/c35bfe7c51a0bce6fe7439d58f5fecda9980a5dd/src/Autofac.Extras.DynamicProxy/RegistrationExtensions.cs </remarks>>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "ReflectionActivator is used to create a ComponentRegistration which then owns the former, and has a lifetime extending outside of this method")]
        private IComponentRegistration CreateClassInterceptorRegistration(IComponentRegistration registration)
        {
            if (registration == null) throw new ArgumentNullException(nameof(registration));

            var existingActivator = registration.Activator as ReflectionActivator;
            var existingImplementationType = existingActivator.LimitType;

            var options = new ProxyGenerationOptions(CreateProxyGenerationHook(existingImplementationType));

            var newImplementationType = ProxyGenerator.ProxyBuilder.CreateClassProxyType(
                existingImplementationType,
                Array.Empty<Type>(),
                options);

            var newActivator = new ReflectionActivator(
                newImplementationType,
                existingActivator.ConstructorFinder,
                existingActivator.ConstructorSelector,
                Enumerable.Empty<Parameter>(),
                Enumerable.Empty<Parameter>());

            var newRegistration = new ComponentRegistration(Guid.NewGuid(), newActivator, registration.Lifetime, registration.Sharing, registration.Ownership, registration.Services, registration.Metadata, registration);
            newRegistration.Preparing += (sender, args) =>
            {
                var proxyParameters = new List<Parameter>();
                int index = 0;

                if (options.HasMixins)
                {
                    foreach (var mixin in options.MixinData.Mixins)
                    {
                        proxyParameters.Add(new PositionalParameter(index++, mixin));
                    }
                }

                var interceptors = GetInterceptorServices(args.Component)
                    .Select(s => args.Context.ResolveService(s))
                    .Cast<Castle.DynamicProxy.IInterceptor>()
                    .ToArray();
                proxyParameters.Add(new PositionalParameter(index++, interceptors));

                if (options.Selector != null)
                {
                    proxyParameters.Add(new PositionalParameter(index, options.Selector));
                }

                args.Parameters = proxyParameters.Concat(args.Parameters).ToArray();
            };

            return newRegistration;
        }

        private bool HasEligibleActivator(IComponentRegistration registration)
        {
            var reflectionActivator = registration.Activator as ReflectionActivator;
            if (reflectionActivator == null) return false;

            return ShouldInterceptType(reflectionActivator.LimitType);
        }

        private static bool IsChildRegistration(IComponentRegistration registration)
        {
            return (registration != registration.Target) && (GetChildRegistration(registration.Target) == registration);
        }

        private static IComponentRegistration GetChildRegistration(IComponentRegistration registration)
        {
            registration.Metadata.TryGetValue(ChildRegistrationPropertyName, out var childRegistration);
            return childRegistration as IComponentRegistration;
        }

        private static void SetChildRegistration(IComponentRegistration registration, IComponentRegistration childRegistration)
        {
            registration.Metadata.Add(ChildRegistrationPropertyName, childRegistration);
        }

        private static IEnumerable<Service> GetInterceptorServices(IComponentRegistration childRegistration)
        {
            if (!childRegistration.Metadata.TryGetValue(InterceptorServicesPropertyName, out var collection))
            {
                return Enumerable.Empty<Service>();
            }

            return collection as IEnumerable<Service>;
        }

        private static void SetInterceptorServices(IComponentRegistration childRegistration, IEnumerable<Service> interceptorServices)
        {
            childRegistration.Metadata[InterceptorServicesPropertyName] = interceptorServices.ToArray();
        }
    }
}
