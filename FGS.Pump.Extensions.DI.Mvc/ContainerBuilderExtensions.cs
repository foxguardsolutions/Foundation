using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Mvc.Filters;

using Autofac;
using Autofac.Builder;
using Autofac.Core;

namespace FGS.Pump.Extensions.DI.Mvc
{
    public static class ContainerBuilderExtensions
    {
        public static void RegisterResultFilterOverrideForWhen(this ContainerBuilder builder, Func<ControllerContext, ActionDescriptor, bool> filterCondition, FilterScope filterScope)
        {
            RegisterFilterOverrideForWhen<IResultFilter>(builder, CustomAutofacFilterProvider.ResultFilterOverrideMetadataKey, filterCondition, filterScope);
        }

        public static void RegisterActionFilterOverrideForWhen(this ContainerBuilder builder, Func<ControllerContext, ActionDescriptor, bool> filterCondition, FilterScope filterScope)
        {
            RegisterFilterOverrideForWhen<IActionFilter>(builder, CustomAutofacFilterProvider.ActionFilterOverrideMetadataKey, filterCondition, filterScope);
        }

        public static void RegisterAuthenticationFilterOverrideForWhen(this ContainerBuilder builder, Func<ControllerContext, ActionDescriptor, bool> filterCondition, FilterScope filterScope)
        {
            RegisterFilterOverrideForWhen<IAuthenticationFilter>(builder, CustomAutofacFilterProvider.AuthenticationFilterOverrideMetadataKey, filterCondition, filterScope);
        }

        public static void RegisterAuthorizationFilterOverrideForWhen(this ContainerBuilder builder, Func<ControllerContext, ActionDescriptor, bool> filterCondition, FilterScope filterScope)
        {
            RegisterFilterOverrideForWhen<IAuthorizationFilter>(builder, CustomAutofacFilterProvider.AuthorizationFilterOverrideMetadataKey, filterCondition, filterScope);
        }

        public static void RegisterExceptionFilterOverrideForWhen(this ContainerBuilder builder, Func<ControllerContext, ActionDescriptor, bool> filterCondition, FilterScope filterScope)
        {
            RegisterFilterOverrideForWhen<IExceptionFilter>(builder, CustomAutofacFilterProvider.ExceptionFilterOverrideMetadataKey, filterCondition, filterScope);
        }

        public static IRegistrationBuilder<TLimit, TActivatorData, TStyle> WithPropertyValueFromControllerAttribute<TLimit, TActivatorData, TStyle, TAttribute, TParameterValue>(this IRegistrationBuilder<TLimit, TActivatorData, TStyle> registration, string propertyName, Func<TAttribute, TParameterValue> parameterValueResolver)
        {
            return registration.OnActivating(
                aea =>
                    {
                        var valueForProperty = GetValueFromControllerAttribute(aea.Parameters, parameterValueResolver);
                        aea.Instance.GetType().GetProperty(propertyName).GetSetMethod().Invoke(aea.Instance, new object[] { valueForProperty });
                    });
        }

        public static IRegistrationBuilder<TLimit, TActivatorData, TStyle> WithPropertyValueFromActionAttribute<TLimit, TActivatorData, TStyle, TAttribute, TParameterValue>(this IRegistrationBuilder<TLimit, TActivatorData, TStyle> registration, string propertyName, Func<TAttribute, TParameterValue> parameterValueResolver)
        {
            return registration.OnActivating(
                aea =>
                {
                    var valueForProperty = GetValueFromActionAttribute(aea.Parameters, parameterValueResolver);
                    aea.Instance.GetType().GetProperty(propertyName).GetSetMethod().Invoke(aea.Instance, new object[] { valueForProperty });
                });
        }

        public static IRegistrationBuilder<TLimit, TActivatorData, TStyle> WithParameterFromControllerAttribute<TLimit, TActivatorData, TStyle, TAttribute, TParameterValue>(this IRegistrationBuilder<TLimit, TActivatorData, TStyle> registration, string parameterName, Func<TAttribute, TParameterValue> parameterValueResolver)
            where TActivatorData : ReflectionActivatorData
        {
            return registration.OnPreparing(
                pea =>
                {
                    var parameterValue = GetValueFromControllerAttribute(pea.Parameters, parameterValueResolver);
                    var newParameter = new ResolvedParameter((pi, ctx) => pi.Name == parameterName && pi.ParameterType.IsAssignableFrom(typeof(TParameterValue)), (pi, ctx) => parameterValue);
                    pea.Parameters = pea.Parameters.Concat(new[] { newParameter });
                });
        }

        public static IRegistrationBuilder<TLimit, TActivatorData, TStyle> WithParameterFromActionAttribute<TLimit, TActivatorData, TStyle, TAttribute, TParameterValue>(this IRegistrationBuilder<TLimit, TActivatorData, TStyle> registration, string parameterName, Func<TAttribute, TParameterValue> parameterValueResolver)
            where TActivatorData : ReflectionActivatorData
        {
            return registration.OnPreparing(
                pea =>
                {
                    var parameterValue = GetValueFromActionAttribute(pea.Parameters, parameterValueResolver);
                    var newParameter = new ResolvedParameter((pi, ctx) => pi.Name == parameterName && pi.ParameterType.IsAssignableFrom(typeof(TParameterValue)), (pi, ctx) => parameterValue);
                    pea.Parameters = pea.Parameters.Concat(new[] { newParameter });
                });
        }

        private static void RegisterFilterOverrideForWhen<TOverriddenFilter>(ContainerBuilder builder, string metadataKey, Func<ControllerContext, ActionDescriptor, bool> filterCondition, FilterScope filterScope)
        {
            var filterMetadata = new CustomFilterMetadata(filterCondition, filterScope, order: default(int));
            builder.RegisterInstance(new CustomAutofacOverrideFilter(typeof(TOverriddenFilter))).As<IOverrideFilter>().WithMetadata(metadataKey, filterMetadata);
        }

        private static TValue GetValueFromControllerAttribute<TAttribute, TValue>(IEnumerable<Parameter> currentParameters, Func<TAttribute, TValue> valueResolver)
        {
            var controllerContextParameter = currentParameters.OfType<NamedParameter>().Single(p => p.Name == CustomAutofacFilterProvider.ControllerContextParameterName);
            var controllerContext = controllerContextParameter.Value as ControllerContext;
            var attribute = controllerContext.Controller.GetType().GetCustomAttributes(typeof(TAttribute), true).OfType<TAttribute>().Last();
            var valueForProperty = valueResolver(attribute);
            return valueForProperty;
        }

        private static TValue GetValueFromActionAttribute<TAttribute, TValue>(IEnumerable<Parameter> currentParameters, Func<TAttribute, TValue> valueResolver)
        {
            var actionDescriptorParameter = currentParameters.OfType<NamedParameter>().Single(p => p.Name == CustomAutofacFilterProvider.ActionDescriptorParameterName);
            var actionDescriptor = actionDescriptorParameter.Value as ActionDescriptor;
            var attribute = actionDescriptor.GetCustomAttributes(typeof(TAttribute), true).OfType<TAttribute>().Last();
            var valueForProperty = valueResolver(attribute);
            return valueForProperty;
        }
    }
}
