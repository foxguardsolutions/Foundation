using System;

using Autofac;
using Autofac.Builder;

namespace FGS.Autofac.Registration.Extensions
{
    /// <summary>
    /// Extends <see cref="IRegistrationBuilder{TLimit, TReflectionActivatorData, TStyle}"/> with functionality that makes parameterizing resolved instances easier to configure.
    /// </summary>
    public static class RegistrationBuilderExtensions
    {
        /// <summary>
        /// Augments the registration <paramref name="registration"/> to contain a parameter that will be matched to a target
        /// based on its type <typeparamref name="TParameter"/>, and resolved using <paramref name="valueProvider"/>.
        /// </summary>
        /// <param name="registration">The registration into which the parameter is being added.</param>
        /// <param name="valueProvider">The factory that creates or retrieves the value of the parameter being added.</param>
        /// <typeparam name="TLimit">Registration limit type.</typeparam>
        /// <typeparam name="TReflectionActivatorData">Activator data type.</typeparam>
        /// <typeparam name="TStyle">Registration style.</typeparam>
        /// <typeparam name="TParameter">The type of the value of the parameter being added.</typeparam>
        /// <returns>The registration, so that additional configuration calls can be chained.</returns>
        public static IRegistrationBuilder<TLimit, TReflectionActivatorData, TStyle> WithParameterTypedFrom<TLimit, TReflectionActivatorData, TStyle, TParameter>(
            this IRegistrationBuilder<TLimit, TReflectionActivatorData, TStyle> registration,
            Func<IComponentContext, TParameter> valueProvider)
            where TReflectionActivatorData : ReflectionActivatorData
        {
            return registration.WithParameter((pi, ctx) => pi.ParameterType.IsAssignableFrom(typeof(TParameter)), (pi, ctx) => valueProvider(ctx));
        }

        /// <summary>
        /// Augments the registration <paramref name="registration"/> to contain a parameter that will be matched to a target
        /// based on its type <typeparamref name="TParameter"/>, resolved using <paramref name="valueProvider"/>, and named <paramref name="name"/>.
        /// </summary>
        /// <param name="registration">The registration into which the parameter is being added.</param>
        /// <param name="name">The name of the parameter being added.</param>
        /// <param name="valueProvider">The factory that creates or retrieves the value of the parameter being added.</param>
        /// <typeparam name="TLimit">Registration limit type.</typeparam>
        /// <typeparam name="TReflectionActivatorData">Activator data type.</typeparam>
        /// <typeparam name="TStyle">Registration style.</typeparam>
        /// <typeparam name="TParameter">The type of the value of the parameter being added.</typeparam>
        /// <returns>The registration, so that additional configuration calls can be chained.</returns>
        public static IRegistrationBuilder<TLimit, TReflectionActivatorData, TStyle> WithParameterTypedFromAndNamed<TLimit, TReflectionActivatorData, TStyle, TParameter>(
            this IRegistrationBuilder<TLimit, TReflectionActivatorData, TStyle> registration,
            string name,
            Func<IComponentContext, TParameter> valueProvider)
            where TReflectionActivatorData : ReflectionActivatorData
        {
            return registration.WithParameter((pi, ctx) => pi.ParameterType.IsAssignableFrom(typeof(TParameter)) && pi.Name == name, (pi, ctx) => valueProvider(ctx));
        }

        /// <summary>
        /// Augments the registration <paramref name="registration"/> to contain a parameter that will be matched to a target
        /// property based on its name <paramref name="propertyName"/>, resolved using <paramref name="propertyValueResolver"/>.
        /// </summary>
        /// <param name="registration">The registration into which the parameter is being added.</param>
        /// <param name="propertyName">The name of the property the parameter is for.</param>
        /// <param name="propertyValueResolver">The factory that creates or retrieves the value of the parameter being added.</param>
        /// <typeparam name="TLimit">Registration limit type.</typeparam>
        /// <typeparam name="TActivatorData">Activator data type.</typeparam>
        /// <typeparam name="TStyle">Registration style.</typeparam>
        /// <typeparam name="TPropertyValue">The type of the value of the parameter being added. Should be assignable to the target property.</typeparam>
        /// <returns>The registration, so that additional configuration calls can be chained.</returns>
        public static IRegistrationBuilder<TLimit, TActivatorData, TStyle> WithProperty<TLimit, TActivatorData, TStyle, TPropertyValue>(this IRegistrationBuilder<TLimit, TActivatorData, TStyle> registration, string propertyName, Func<IComponentContext, TPropertyValue> propertyValueResolver)
            where TActivatorData : ReflectionActivatorData
        {
            return registration.WithProperty(new ResolvedNamedPropertyParameter<TPropertyValue>(propertyName, propertyValueResolver));
        }
    }
}
