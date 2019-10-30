using System;
using System.Diagnostics.CodeAnalysis;

using Autofac;

using FGS.Autofac.DynamicScoping;
using FGS.Autofac.DynamicScoping.Abstractions;

using Moq;

namespace FGS.Tests.Support.Autofac.Mocking
{
    public static class ContainerBuilderExtensions
    {
        public static Mock<T> RegisterMock<T>(this ContainerBuilder builder, Scope scope)
            where T : class
        {
            var mock = new Mock<T>();
            builder.Register(ctx => mock.Object).AsSelf().In(scope);
            return mock;
        }

        public static void RegisterUnboundMock(this ContainerBuilder builder, Type unboundGeneric, Scope scope)
        {
            if (unboundGeneric.IsValueType || !unboundGeneric.IsGenericType || unboundGeneric.IsConstructedGenericType)
                throw new ArgumentOutOfRangeException(nameof(unboundGeneric), "Argument must be an unbound generic reference type");

            builder.Register(ctx => new MockRepository(MockBehavior.Default)).AsSelf().IfNotRegistered(typeof(MockRepository)).In(scope);

            builder.RegisterCallback(cr => cr.AddRegistrationSource(new UnboundMockRegistrationSource(unboundGeneric)));
        }
    }
}
