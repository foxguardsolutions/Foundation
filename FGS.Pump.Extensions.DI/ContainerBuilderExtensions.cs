using System;

using Autofac;
using Autofac.Builder;
using Autofac.Core;

namespace FGS.Pump.Extensions.DI
{
    public static class ContainerBuilderExtensions
    {
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
            }
        }
