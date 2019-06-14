using System;

using Autofac;
using Autofac.Builder;

namespace FGS.Pump.Extensions.DI
{
    public static class RegistrationBuilderExtensions
    {
        public static IRegistrationBuilder<TLimit, TReflectionActivatorData, TStyle> WithParameterTypedFrom<TLimit, TReflectionActivatorData, TStyle, TParameter>(
            this IRegistrationBuilder<TLimit, TReflectionActivatorData, TStyle> registration,
            Func<IComponentContext, TParameter> valueProvider)
            where TReflectionActivatorData : ReflectionActivatorData
        {
            return registration.WithParameter((pi, ctx) => pi.ParameterType.IsAssignableFrom(typeof(TParameter)), (pi, ctx) => valueProvider(ctx));
        }

        public static IRegistrationBuilder<TLimit, TReflectionActivatorData, TStyle> WithParameterTypedFromAndNamed<TLimit, TReflectionActivatorData, TStyle, TParameter>(
            this IRegistrationBuilder<TLimit, TReflectionActivatorData, TStyle> registration,
            string name,
            Func<IComponentContext, TParameter> valueProvider)
            where TReflectionActivatorData : ReflectionActivatorData
        {
            return registration.WithParameter((pi, ctx) => pi.ParameterType.IsAssignableFrom(typeof(TParameter)) && pi.Name == name, (pi, ctx) => valueProvider(ctx));
        }

        public static IRegistrationBuilder<TLimit, TActivatorData, TStyle> WithProperty<TLimit, TActivatorData, TStyle, TPropertyValue>(this IRegistrationBuilder<TLimit, TActivatorData, TStyle> builder, string propertyName, Func<IComponentContext, TPropertyValue> propertyValueResolver)
            where TActivatorData : ReflectionActivatorData
        {
            return builder.WithProperty(new ResolvedNamedPropertyParameter<TPropertyValue>(propertyName, propertyValueResolver));
        }
    }
}
