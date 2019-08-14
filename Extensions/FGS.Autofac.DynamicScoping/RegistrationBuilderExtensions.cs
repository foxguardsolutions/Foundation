using System;

using Autofac;
using Autofac.Builder;

using FGS.Autofac.DynamicScoping.Abstractions;

namespace FGS.Autofac.DynamicScoping
{
    public static class RegistrationBuilderExtensions
    {
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
