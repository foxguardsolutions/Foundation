using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

using Autofac;
using Autofac.Builder;
using Autofac.Core;

namespace FGS.Pump.Extensions.DI.WebApi
{
    public static class RegistrationBuilderExtensions
    {
        public static IRegistrationBuilder<TLimit, TActivatorData, TStyle> AsWebApiAuthorizationFilterWhen<TLimit, TActivatorData, TStyle>(this IRegistrationBuilder<TLimit, TActivatorData, TStyle> registration, Func<HttpControllerDescriptor, HttpActionDescriptor, bool> filterCondition, FilterScope filterScope, int order)
            where TLimit : ICustomAutofacAuthorizationFilter
        {
            return registration.AsFilterWhen<TLimit, TActivatorData, TStyle, ICustomAutofacAuthorizationFilter>(filterCondition, filterScope, CustomAutofacWebApiFilterProvider.AuthorizationFilterMetadataKey, order);
        }

        public static IRegistrationBuilder<TLimit, TActivatorData, TStyle> AsWebApiActionFilterWhen<TLimit, TActivatorData, TStyle>(this IRegistrationBuilder<TLimit, TActivatorData, TStyle> registration, Func<HttpControllerDescriptor, HttpActionDescriptor, bool> filterCondition, FilterScope filterScope, int order)
            where TLimit : ICustomAutofacActionFilter
        {
            return registration.AsFilterWhen<TLimit, TActivatorData, TStyle, ICustomAutofacActionFilter>(filterCondition, filterScope, CustomAutofacWebApiFilterProvider.ActionFilterMetadataKey, order);
        }

        public static IRegistrationBuilder<TLimit, TActivatorData, TStyle> AsWebApiAuthenticationFilterWhen<TLimit, TActivatorData, TStyle>(this IRegistrationBuilder<TLimit, TActivatorData, TStyle> registration, Func<HttpControllerDescriptor, HttpActionDescriptor, bool> filterCondition, FilterScope filterScope, int order)
            where TLimit : ICustomAutofacAuthenticationFilter
        {
            return registration.AsFilterWhen<TLimit, TActivatorData, TStyle, ICustomAutofacAuthenticationFilter>(filterCondition, filterScope, CustomAutofacWebApiFilterProvider.AuthenticationFilterMetadataKey, order);
        }

        public static IRegistrationBuilder<TLimit, TActivatorData, TStyle> AsWebApiExceptionFilterWhen<TLimit, TActivatorData, TStyle>(this IRegistrationBuilder<TLimit, TActivatorData, TStyle> registration, Func<HttpControllerDescriptor, HttpActionDescriptor, bool> filterCondition, FilterScope filterScope, int order)
            where TLimit : ICustomAutofacExceptionFilter
        {
            return registration.AsFilterWhen<TLimit, TActivatorData, TStyle, ICustomAutofacExceptionFilter>(filterCondition, filterScope, CustomAutofacWebApiFilterProvider.ExceptionFilterMetadataKey, order);
        }

        public static IRegistrationBuilder<TLimit, TActivatorData, TStyle> AsWebApiAuthorizationFilterOverrideWhen<TLimit, TActivatorData, TStyle>(this IRegistrationBuilder<TLimit, TActivatorData, TStyle> registration, Func<HttpControllerDescriptor, HttpActionDescriptor, bool> filterCondition, FilterScope filterScope, int order)
            where TLimit : ICustomAutofacAuthorizationFilter
        {
            return registration.AsFilterWhen<TLimit, TActivatorData, TStyle, ICustomAutofacAuthorizationFilter>(filterCondition, filterScope, CustomAutofacWebApiFilterProvider.AuthorizationFilterOverrideMetadataKey, order);
        }

        public static IRegistrationBuilder<TLimit, TActivatorData, TStyle> AsWebApiActionFilterOverrideWhen<TLimit, TActivatorData, TStyle>(this IRegistrationBuilder<TLimit, TActivatorData, TStyle> registration, Func<HttpControllerDescriptor, HttpActionDescriptor, bool> filterCondition, FilterScope filterScope, int order)
            where TLimit : ICustomAutofacActionFilter
        {
            return registration.AsFilterWhen<TLimit, TActivatorData, TStyle, ICustomAutofacActionFilter>(filterCondition, filterScope, CustomAutofacWebApiFilterProvider.ActionFilterOverrideMetadataKey, order);
        }

        public static IRegistrationBuilder<TLimit, TActivatorData, TStyle> AsWebApiAuthenticationFilterOverrideWhen<TLimit, TActivatorData, TStyle>(this IRegistrationBuilder<TLimit, TActivatorData, TStyle> registration, Func<HttpControllerDescriptor, HttpActionDescriptor, bool> filterCondition, FilterScope filterScope, int order)
            where TLimit : ICustomAutofacAuthenticationFilter
        {
            return registration.AsFilterWhen<TLimit, TActivatorData, TStyle, ICustomAutofacAuthenticationFilter>(filterCondition, filterScope, CustomAutofacWebApiFilterProvider.AuthenticationFilterOverrideMetadataKey, order);
        }

        public static IRegistrationBuilder<TLimit, TActivatorData, TStyle> AsWebApiExceptionFilterOverrideWhen<TLimit, TActivatorData, TStyle>(this IRegistrationBuilder<TLimit, TActivatorData, TStyle> registration, Func<HttpControllerDescriptor, HttpActionDescriptor, bool> filterCondition, FilterScope filterScope, int order)
            where TLimit : ICustomAutofacExceptionFilter
        {
            return registration.AsFilterWhen<TLimit, TActivatorData, TStyle, ICustomAutofacExceptionFilter>(filterCondition, filterScope, CustomAutofacWebApiFilterProvider.ExceptionFilterOverrideMetadataKey, order);
        }

        public static IRegistrationBuilder<TLimit, TActivatorData, TStyle> WithPropertyValueFromApiControllerAttribute<TLimit, TActivatorData, TStyle, TAttribute, TParameterValue>(this IRegistrationBuilder<TLimit, TActivatorData, TStyle> registration, string propertyName, Func<TAttribute, TParameterValue> parameterValueResolver)
            where TAttribute : class
        {
            return registration.OnActivating(
                aea =>
                {
                    var valueForProperty = GetValueFromControllerAttribute(aea.Parameters, parameterValueResolver);
                    aea.Instance.GetType().GetProperty(propertyName).GetSetMethod().Invoke(aea.Instance, new object[] { valueForProperty });
                });
        }

        public static IRegistrationBuilder<TLimit, TActivatorData, TStyle> WithPropertyValueFromApiActionAttribute<TLimit, TActivatorData, TStyle, TAttribute, TParameterValue>(this IRegistrationBuilder<TLimit, TActivatorData, TStyle> registration, string propertyName, Func<TAttribute, TParameterValue> parameterValueResolver)
            where TAttribute : class
        {
            return registration.OnActivating(
                aea =>
                {
                    var valueForProperty = GetValueFromActionAttribute(aea.Parameters, parameterValueResolver);
                    aea.Instance.GetType().GetProperty(propertyName).GetSetMethod().Invoke(aea.Instance, new object[] { valueForProperty });
                });
        }

        public static IRegistrationBuilder<TLimit, TActivatorData, TStyle> WithParameterFromApiControllerAttribute<TLimit, TActivatorData, TStyle, TAttribute, TParameterValue>(this IRegistrationBuilder<TLimit, TActivatorData, TStyle> registration, string parameterName, Func<TAttribute, TParameterValue> parameterValueResolver)
            where TActivatorData : ReflectionActivatorData
            where TAttribute : class
        {
            return registration.OnPreparing(
                pea =>
                {
                    var parameterValue = GetValueFromControllerAttribute(pea.Parameters, parameterValueResolver);
                    var newParameter = new ResolvedParameter((pi, ctx) => pi.Name == parameterName && pi.ParameterType.IsAssignableFrom(typeof(TParameterValue)), (pi, ctx) => parameterValue);
                    pea.Parameters = pea.Parameters.Concat(new[] { newParameter });
                });
        }

        public static IRegistrationBuilder<TLimit, TActivatorData, TStyle> WithParameterFromApiActionAttribute<TLimit, TActivatorData, TStyle, TAttribute, TParameterValue>(this IRegistrationBuilder<TLimit, TActivatorData, TStyle> registration, string parameterName, Func<TAttribute, TParameterValue> parameterValueResolver)
            where TActivatorData : ReflectionActivatorData
            where TAttribute : class
        {
            return registration.OnPreparing(
                pea =>
                {
                    var parameterValue = GetValueFromActionAttribute(pea.Parameters, parameterValueResolver);
                    var newParameter = new ResolvedParameter((pi, ctx) => pi.Name == parameterName && pi.ParameterType.IsAssignableFrom(typeof(TParameterValue)), (pi, ctx) => parameterValue);
                    pea.Parameters = pea.Parameters.Concat(new[] { newParameter });
                });
        }

        private static IRegistrationBuilder<TLimit, TActivatorData, TStyle> AsFilterWhen<TLimit, TActivatorData, TStyle, TFilterKind>(this IRegistrationBuilder<TLimit, TActivatorData, TStyle> registration, Func<HttpControllerDescriptor, HttpActionDescriptor, bool> filterCondition, FilterScope filterScope, string metadataKey, int order)
            where TLimit : TFilterKind
        {
            var metadata = new CustomWebApiFilterMetadata()
            {
                Predicate = filterCondition,
                FilterScope = filterScope,
                Order = order
            };
            return registration.As<TFilterKind>().WithMetadata(metadataKey, metadata);
        }

        private static TValue GetValueFromControllerAttribute<TAttribute, TValue>(IEnumerable<Parameter> currentParameters, Func<TAttribute, TValue> valueResolver)
            where TAttribute : class
        {
            var controllerDescriptorParameter = currentParameters.OfType<NamedParameter>().Single(p => p.Name == HttpActionDescriptorExtensions.HttpControllerDescriptorParameterName);
            var controllerDescriptor = controllerDescriptorParameter.Value as HttpControllerDescriptor;
            var attribute = controllerDescriptor.GetCustomAttributes<TAttribute>().Last();
            var valueForProperty = valueResolver(attribute);
            return valueForProperty;
        }

        private static TValue GetValueFromActionAttribute<TAttribute, TValue>(IEnumerable<Parameter> currentParameters, Func<TAttribute, TValue> valueResolver)
            where TAttribute : class
        {
            var actionDescriptorParameter = currentParameters.OfType<NamedParameter>().Single(p => p.Name == HttpActionDescriptorExtensions.HttpActionDescriptorParameterName);
            var actionDescriptor = actionDescriptorParameter.Value as HttpActionDescriptor;
            var attribute = actionDescriptor.GetCustomAttributes<TAttribute>().Last();
            var valueForProperty = valueResolver(attribute);
            return valueForProperty;
        }
    }
}
