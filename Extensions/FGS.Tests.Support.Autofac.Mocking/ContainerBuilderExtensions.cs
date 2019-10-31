using System;

using Autofac;

using FGS.Autofac.DynamicScoping;
using FGS.Autofac.DynamicScoping.Abstractions;

using Moq;

namespace FGS.Tests.Support.Autofac.Mocking
{
    /// <summary>
    /// Extends <see cref="ContainerBuilder"/> with functionality for registering mocks.
    /// </summary>
    public static class ContainerBuilderExtensions
    {
        /// <summary>
        /// Registers a mocked <typeparamref name="T"/> with the <paramref name="builder"/>.
        /// </summary>
        /// <typeparam name="T">The type of object to mock.</typeparam>
        /// <param name="builder">The <see cref="ContainerBuilder"/> into which to register the mock.</param>
        /// <param name="scope">The lifetime management semantics by which the component will be resolved.</param>
        /// <returns>A <see cref="Mock{T}"/> so that additional configuration calls may be chained.</returns>
        /// <remarks>The mocked object is registered with a deferred style, such that callers may continue to configure the mock until it is resolved.</remarks>
        public static Mock<T> RegisterMock<T>(this ContainerBuilder builder, Scope scope)
            where T : class
        {
            var mock = new Mock<T>();
            builder.Register(ctx => mock.Object).AsSelf().In(scope);
            return mock;
        }

        /// <summary>
        /// Registers mocks of the unbound generic type represented by <paramref name="unboundGeneric"/>.
        /// </summary>
        /// <param name="builder">The <see cref="ContainerBuilder"/> into which to register the mocks.</param>
        /// <param name="unboundGeneric">The type of unbound generic to generate mock registrations for.</param>
        /// <param name="scope">The lifetime management semantics by which the component will be resolved.</param>
        public static void RegisterUnboundMock(this ContainerBuilder builder, Type unboundGeneric, Scope scope)
        {
            if (unboundGeneric.IsValueType || !unboundGeneric.IsGenericType || unboundGeneric.IsConstructedGenericType)
                throw new ArgumentOutOfRangeException(nameof(unboundGeneric), "Argument must be an unbound generic reference type");

            builder.Register(ctx => new MockRepository(MockBehavior.Default)).AsSelf().IfNotRegistered(typeof(MockRepository)).In(scope);

            builder.RegisterCallback(cr => cr.AddRegistrationSource(new UnboundMockRegistrationSource(unboundGeneric)));
        }
    }
}
