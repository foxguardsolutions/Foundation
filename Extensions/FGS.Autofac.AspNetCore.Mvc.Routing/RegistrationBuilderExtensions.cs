using System;

using Autofac;
using Autofac.Builder;

using FGS.Pump.Extensions.DI;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FGS.Autofac.AspNetCore.Mvc.Routing
{
    public static class RegistrationBuilderExtensions
    {
        public static IRegistrationBuilder<TLimit, TReflectionActivatorData, TStyle> WithSingletonCompatibleUrlHelper<TLimit, TReflectionActivatorData, TStyle>(
            this IRegistrationBuilder<TLimit, TReflectionActivatorData, TStyle> registration)
            where TReflectionActivatorData : ReflectionActivatorData
        {
            return registration.WithParameterTypedFrom(ctx =>
            {
                var lazyHttpContextAccessor = ctx.Resolve<Lazy<IHttpContextAccessor>>();
                var lazyLinkGenerator = ctx.Resolve<Lazy<LinkGenerator>>();

                return (IUrlHelper)new SingletonCompatibleUrlHelper(lazyHttpContextAccessor.Value, lazyLinkGenerator.Value);
            });
        }
    }
}
