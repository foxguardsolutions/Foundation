using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Autofac.Features.Metadata;
using Autofac.Features.OwnedInstances;

using Moq;

namespace FGS.Tests.Support.Autofac.Mocking
{
    /// <summary>
    /// Resolves interfaces matching a given unbound service type, as mocked objects that are created using the <see cref="MockRepository"/> from the scope.
    /// </summary>
    /// <remarks>Taken and modified from: https://github.com/autofac/Autofac.Extras.Moq/blob/3ad044e343e817aa4237d8b6b652860825f832a9/src/Autofac.Extras.Moq/MoqRegistrationHandler.cs. </remarks>
    internal sealed class UnboundMockRegistrationSource : IRegistrationSource
    {
        private readonly Type _unboundMockedServiceType;
        private readonly MethodInfo _createMethod;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnboundMockRegistrationSource"/> class.
        /// </summary>
        /// <param name="unboundMockedServiceType">The unbound type service for which to resolve mocked instances of.</param>
        internal UnboundMockRegistrationSource(Type unboundMockedServiceType)
        {
            if (unboundMockedServiceType.IsValueType || !unboundMockedServiceType.IsGenericType || unboundMockedServiceType.IsConstructedGenericType)
                throw new ArgumentOutOfRangeException(nameof(unboundMockedServiceType), "Argument must be an unbound generic reference type");

            _unboundMockedServiceType = unboundMockedServiceType;

            // This is MockRepository.Create<T>() with zero parameters. This is important because
            // it limits what can be auto-mocked.
            var factoryType = typeof(MockRepository);
            this._createMethod = factoryType.GetMethod(nameof(MockRepository.Create), Array.Empty<Type>());
        }

        /// <summary>
        /// Gets a value indicating whether the registrations provided by
        /// this source are 1:1 adapters on top of other components (i.e. like Meta, Func or Owned).
        /// </summary>
        /// <value>
        /// Always returns <see langword="false" />.
        /// </value>
        public bool IsAdapterForIndividualComponents => false;

        /// <summary>
        /// Retrieve a registration for an service, to be used by the container.
        /// </summary>
        /// <param name="service">The service that was requested.</param>
        /// <param name="registrationAccessor">Not used; required by the interface.</param>
        /// <returns>
        /// Registrations for the service.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown if <paramref name="service" /> is <see langword="null" />.
        /// </exception>
        public IEnumerable<IComponentRegistration> RegistrationsFor(
            Service service,
            Func<Service, IEnumerable<IComponentRegistration>> registrationAccessor)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            var typedService = service as TypedService;
            if (typedService == null || !this.CanMockService(typedService))
            {
                return Enumerable.Empty<IComponentRegistration>();
            }

            var rb = RegistrationBuilder.ForDelegate((c, p) => this.CreateMock(c, typedService))
                .As(service)
                .InstancePerLifetimeScope();

            return new[] { rb.CreateRegistration() };
        }

        private static bool IsIEnumerable(IServiceWithType typedService)
        {
            // We handle most generics, but we don't handle IEnumerable because that has special
            // meaning in Autofac
            return typedService.ServiceType.GetTypeInfo().IsGenericType &&
                   typedService.ServiceType.GetTypeInfo().GetGenericTypeDefinition() == typeof(IEnumerable<>);
        }

        private static bool IsIStartable(IServiceWithType typedService)
        {
            return typeof(IStartable).IsAssignableFrom(typedService.ServiceType);
        }

        private static bool ServiceCompatibleWithMockRepositoryCreate(IServiceWithType typedService)
        {
            var serverTypeInfo = typedService.ServiceType.GetTypeInfo();

            // autofac/Autofac.Extras.Moq Issue 15: Ensure there's a zero-parameter ctor or the DynamicProxy under Moq fails. https://github.com/autofac/Autofac.Extras.Moq/issues/15
            return serverTypeInfo.IsInterface
                   || serverTypeInfo.IsAbstract
                   || (serverTypeInfo.IsClass &&
                       !serverTypeInfo.IsSealed &&
                       typedService.ServiceType.GetConstructors().Any(c => c.GetParameters().Length == 0));
        }

        private bool CanMockService(IServiceWithType typedService)
        {
            // Since we're calling MockRepository.Create<T>() to auto-mock and we don't provide
            // parameter support, it means we're limited to only auto-mocking things that can pass
            // through Moq / Castle.DynamicProxy without any parameters
            return IsUnboundMockedServiceType(typedService) &&
                   ServiceCompatibleWithMockRepositoryCreate(typedService) &&
                   !IsIEnumerable(typedService) &&
                   !IsIStartable(typedService) &&
                   !IsLazy(typedService) &&
                   !IsOwned(typedService) &&
                   !IsMeta(typedService);
        }

        private bool IsUnboundMockedServiceType(IServiceWithType typedService)
        {
            return typedService.ServiceType.IsGenericType &&
                   _unboundMockedServiceType == typedService.ServiceType.GetGenericTypeDefinition();
        }

        private static bool IsLazy(IServiceWithType typedService)
        {
            // We handle most generics, but we don't handle Lazy because that has special
            // meaning in Autofac
            var typeInfo = typedService.ServiceType.GetTypeInfo();
            return typeInfo.IsGenericType &&
                   typeInfo.GetGenericTypeDefinition() == typeof(Lazy<>);
        }

        private static bool IsOwned(IServiceWithType typedService)
        {
            // We handle most generics, but we don't handle Owned because that has special
            // meaning in Autofac
            var typeInfo = typedService.ServiceType.GetTypeInfo();
            return typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == typeof(Owned<>);
        }

        private static bool IsMeta(IServiceWithType typedService)
        {
            // We handle most generics, but we don't handle Meta because that has special
            // meaning in Autofac
            var typeInfo = typedService.ServiceType.GetTypeInfo();
            return typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == typeof(Meta<>);
        }

        /// <summary>
        /// Creates a mock object.
        /// </summary>
        /// <param name="context">The component context.</param>
        /// <param name="typedService">The typed service.</param>
        /// <returns>
        /// The mock object from the repository.
        /// </returns>
        private object CreateMock(IComponentContext context, IServiceWithType typedService)
        {
            var specificCreateMethod = this._createMethod.MakeGenericMethod(new[] { typedService.ServiceType });
            var mock = (Mock)specificCreateMethod.Invoke(context.Resolve<MockRepository>(), null);
            return mock.Object;
        }
    }
}
