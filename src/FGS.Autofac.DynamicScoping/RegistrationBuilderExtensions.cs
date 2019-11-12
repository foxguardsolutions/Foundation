using System;

using Autofac;
using Autofac.Builder;

using FGS.Autofac.DynamicScoping.Abstractions;

namespace FGS.Autofac.DynamicScoping
{
    /// <summary>
    /// Extends <see cref="IRegistrationBuilder{TLimit, TActivatorData, TStyle} "/> with functionality that allows specififying object lifetime via an enum value.
    /// </summary>
    public static class RegistrationBuilderExtensions
    {
        /// <summary>
        /// Configure the component so that is resolved using the semantics of the scope indicated by <paramref name="scope"/>.
        /// </summary>
        /// <param name="registration">The component registration that is being created.</param>
        /// <param name="scope">Indicates the lifetime management semantics by which the component will be resolved.</param>
        /// <returns>A registration builder allowing further configuration of the component.</returns>
        public static IRegistrationBuilder<TLimit, TActivatorData, TStyle> In<TLimit, TActivatorData, TStyle>(this IRegistrationBuilder<TLimit, TActivatorData, TStyle> registration, Scope scope)
        {
            switch (scope)
            {
                case Scope.PerDependency:
                    return registration.InstancePerDependency();
                case Scope.PerLifetimeScope:
                    return registration.InstancePerLifetimeScope();
                case Scope.Singleton:
                    return registration.SingleInstance();
                default:
                    throw new ArgumentOutOfRangeException(nameof(scope), scope, null);
            }
        }
    }
}
