using System;

using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Autofac.Features.Indexed;

using FGS.Autofac.DynamicScoping;
using FGS.Autofac.DynamicScoping.Abstractions;

namespace FGS.Autofac.Registration.Extensions
{
    /// <summary>
    /// Extends <see cref="ContainerBuilder"/> with helpers that reduce the overhead of certain common registration patterns.
    /// </summary>
    public static class ContainerBuilderExtensions
    {
        /// <summary>
        /// Registers a decorated service - of type <typeparamref name="TImplementation"/> and decorated by type <typeparamref name="TDecorator"/> - as an implementation
        /// of <typeparamref name="TService"/>, in the scope of <paramref name="scope"/>.
        /// </summary>
        /// <param name="builder">The <see cref="ContainerBuilder"/> to add the registrations to.</param>
        /// <param name="scope">Indicates the lifetime management semantics by which the components will be resolved.</param>
        /// <typeparam name="TDecorator">The type that decorates the underlying implementation.</typeparam>
        /// <typeparam name="TImplementation">The underlying implementation.</typeparam>
        /// <typeparam name="TService">The service being implemented.</typeparam>
        /// <returns>The <see cref="IRegistrationBuilder{TService, SimpleActivatorData, SingleRegistrationStyle}"/> of the outer-most component, so that additional registration calls can be chained.</returns>
        public static IRegistrationBuilder<TService, SimpleActivatorData, SingleRegistrationStyle> RegisterDecorator<TDecorator, TImplementation, TService>(this ContainerBuilder builder, Scope scope)
            where TDecorator : TService
            where TImplementation : TService
        {
            var implementationRegistrationName = $"{typeof(TService).Name} {Guid.NewGuid()}";
            var decoratorRegistrationName = $"{typeof(TService).Name} {Guid.NewGuid()}";
            builder.RegisterType<TImplementation>().Named<TImplementation>(implementationRegistrationName).In(scope);
            builder.RegisterType<TDecorator>().Named<TService>(decoratorRegistrationName).In(scope);
            var implementationResolvingParameter = new ResolvedParameter(
                (pi, ctx) => pi.ParameterType.IsAssignableFrom(typeof(TImplementation)),
                (pi, ctx) => ctx.ResolveNamed<TImplementation>(implementationRegistrationName));
            return builder.Register(ctx => ctx.ResolveNamed<TService>(decoratorRegistrationName, implementationResolvingParameter)).As<TService>().In(scope);
        }

        /// <summary>
        /// Registers a doubly-decorated service - of type <typeparamref name="TImplementation"/>, decorated by types <typeparamref name="TDecorator1"/> and <typeparamref name="TDecorator2"/> - as an implementation
        /// of <typeparamref name="TService"/>, in the scope of <paramref name="scope"/>.
        /// </summary>
        /// <param name="builder">The <see cref="ContainerBuilder"/> to add the registrations to.</param>
        /// <param name="scope">Indicates the lifetime management semantics by which the components will be resolved.</param>
        /// <typeparam name="TDecorator2">The outer-most type that decorates components in this registration.</typeparam>
        /// <typeparam name="TDecorator1">The type that decorates the inner-most component in this registration.</typeparam>
        /// <typeparam name="TImplementation">The underlying inner-most implementation.</typeparam>
        /// <typeparam name="TService">The service being implemented.</typeparam>
        /// <returns>The <see cref="IRegistrationBuilder{TService, SimpleActivatorData, SingleRegistrationStyle}"/> of the outer-most component, so that additional registration calls can be chained.</returns>
        public static IRegistrationBuilder<TService, SimpleActivatorData, SingleRegistrationStyle> RegisterDecorator<TDecorator2, TDecorator1, TImplementation, TService>(this ContainerBuilder builder, Scope scope)
            where TDecorator2 : TService
            where TDecorator1 : TService
            where TImplementation : TService
        {
            var implementationRegistrationName = $"{typeof(TService).Name} {Guid.NewGuid()}";

            var decoratorRegistrationName = $"{typeof(TService).Name} {Guid.NewGuid()}";
            builder.RegisterDecorator<TDecorator1, TImplementation, TService>(scope).Named<TService>(implementationRegistrationName);
            builder.RegisterType<TDecorator2>().Named<TService>(decoratorRegistrationName).In(scope);
            var implementationResolvingParameter = new ResolvedParameter(
                (pi, ctx) => pi.ParameterType.IsAssignableFrom(typeof(TService)),
                (pi, ctx) => ctx.ResolveNamed<TService>(implementationRegistrationName));
            return builder.Register(ctx => ctx.ResolveNamed<TService>(decoratorRegistrationName, implementationResolvingParameter)).As<TService>().In(scope);
        }

        /// <summary>
        /// Registers a <see cref="Func{TKey, TService}"/>-based factory as a wrapper around an erstwhile resolvable <see cref="IIndex{TKey, TService}"/>.
        /// This allows consumers to take advantage <see cref="IIndex{TKey, TService}"/> semantics without directly coupling to Autofac's types.
        /// </summary>
        /// <typeparam name="TKey">The type of keys in the index being wrapped.</typeparam>
        /// <typeparam name="TService">The type of services (values) in the index being wrapped.</typeparam>
        /// <param name="builder">The <see cref="ContainerBuilder"/> to add registrations to.</param>
        /// <returns>The <see cref="IRegistrationBuilder{TLimit, TActivationData, TRegistrationStyle}"/>, so that additional registration calls can be chained.</returns>
        public static IRegistrationBuilder<Func<TKey, TService>, SimpleActivatorData, SingleRegistrationStyle> RegisterFactoryFromIndexLookup<TKey, TService>(this ContainerBuilder builder)
        {
            return builder.Register(CreateFactoryFromIndexLookup<TKey, TService>);
        }

        private static Func<TKey, TService> CreateFactoryFromIndexLookup<TKey, TService>(IComponentContext context)
        {
            var index = context.Resolve<IIndex<TKey, TService>>();
            return key => index[key];
        }
    }
}
