using System;

using Autofac;
using Autofac.Builder;

using FGS.Autofac.Registration.Extensions;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FGS.Autofac.AspNetCore.Mvc.Routing
{
    /// <summary>
    /// Extends a <see cref="IRegistrationBuilder{TLimit, TReflectionActivatorData, TStyle}"/> with the capability to provide a singleton-compatible <see cref="IUrlHelper"/>.
    /// </summary>
    public static class RegistrationBuilderExtensions
    {
        /// <summary>
        /// Provides a parameter to <typeparamref name="TLimit"/> that resolves a singleton-compatible <see cref="IUrlHelper"/>.
        /// </summary>
        /// <example>
        /// Use this in an Autofac registration:
        /// <code>
        ///   containerBuilder
        ///         .RegisterType&lt;MyConcreteType&gt;()
        ///         .As&lt;IMyAbstractType&gt;()
        ///         .WithSingletonCompatibleUrlHelper()
        ///         .SingleInstance();
        /// </code>
        /// </example>
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
