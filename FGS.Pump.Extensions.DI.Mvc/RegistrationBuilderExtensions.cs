using System;
using System.Web.Mvc;
using System.Web.Mvc.Filters;

using Autofac.Builder;

namespace FGS.Pump.Extensions.DI.Mvc
{
    public static class RegistrationBuilderExtensions
    {
        public static IRegistrationBuilder<TLimit, TActivatorData, TStyle> AsResultFilterWhen<TLimit, TActivatorData, TStyle>(this IRegistrationBuilder<TLimit, TActivatorData, TStyle> registration, Func<ControllerContext, ActionDescriptor, bool> filterCondition, FilterScope filterScope, int order)
            where TLimit : IResultFilter
        {
            return registration.AsFilterWhen<TLimit, TActivatorData, TStyle, IResultFilter>(filterCondition, filterScope, order, CustomAutofacFilterProvider.ResultFilterMetadataKey);
        }

        public static IRegistrationBuilder<TLimit, TActivatorData, TStyle> AsActionFilterWhen<TLimit, TActivatorData, TStyle>(this IRegistrationBuilder<TLimit, TActivatorData, TStyle> registration, Func<ControllerContext, ActionDescriptor, bool> filterCondition, FilterScope filterScope, int order)
            where TLimit : IActionFilter
        {
            return registration.AsFilterWhen<TLimit, TActivatorData, TStyle, IActionFilter>(filterCondition, filterScope, order, CustomAutofacFilterProvider.ActionFilterMetadataKey);
        }

        public static IRegistrationBuilder<TLimit, TActivatorData, TStyle> AsAuthenticationFilterWhen<TLimit, TActivatorData, TStyle>(this IRegistrationBuilder<TLimit, TActivatorData, TStyle> registration, Func<ControllerContext, ActionDescriptor, bool> filterCondition, FilterScope filterScope, int order)
                    where TLimit : IAuthenticationFilter
        {
            return registration.AsFilterWhen<TLimit, TActivatorData, TStyle, IAuthenticationFilter>(filterCondition, filterScope, order, CustomAutofacFilterProvider.AuthenticationFilterMetadataKey);
        }

        public static IRegistrationBuilder<TLimit, TActivatorData, TStyle> AsAuthorizationFilterWhen<TLimit, TActivatorData, TStyle>(this IRegistrationBuilder<TLimit, TActivatorData, TStyle> registration, Func<ControllerContext, ActionDescriptor, bool> filterCondition, FilterScope filterScope, int order)
            where TLimit : IAuthorizationFilter
        {
            return registration.AsFilterWhen<TLimit, TActivatorData, TStyle, IAuthorizationFilter>(filterCondition, filterScope, order, CustomAutofacFilterProvider.AuthorizationFilterMetadataKey);
        }

        public static IRegistrationBuilder<TLimit, TActivatorData, TStyle> AsExceptionFilterWhen<TLimit, TActivatorData, TStyle>(this IRegistrationBuilder<TLimit, TActivatorData, TStyle> registration, Func<ControllerContext, ActionDescriptor, bool> filterCondition, FilterScope filterScope, int order)
            where TLimit : IExceptionFilter
        {
            return registration.AsFilterWhen<TLimit, TActivatorData, TStyle, IExceptionFilter>(filterCondition, filterScope, order, CustomAutofacFilterProvider.ExceptionFilterMetadataKey);
        }

        public static IRegistrationBuilder<TLimit, TActivatorData, TStyle> AsResultFilterOverrideWhen<TLimit, TActivatorData, TStyle>(this IRegistrationBuilder<TLimit, TActivatorData, TStyle> registration, Func<ControllerContext, ActionDescriptor, bool> filterCondition, FilterScope filterScope, int order)
            where TLimit : IResultFilter
        {
            return registration.AsFilterWhen<TLimit, TActivatorData, TStyle, IResultFilter>(filterCondition, filterScope, order, CustomAutofacFilterProvider.ResultFilterOverrideMetadataKey);
        }

        public static IRegistrationBuilder<TLimit, TActivatorData, TStyle> AsActionFilterOverrideWhen<TLimit, TActivatorData, TStyle>(this IRegistrationBuilder<TLimit, TActivatorData, TStyle> registration, Func<ControllerContext, ActionDescriptor, bool> filterCondition, FilterScope filterScope, int order)
            where TLimit : IActionFilter
        {
            return registration.AsFilterWhen<TLimit, TActivatorData, TStyle, IActionFilter>(filterCondition, filterScope, order, CustomAutofacFilterProvider.ActionFilterOverrideMetadataKey);
        }

        public static IRegistrationBuilder<TLimit, TActivatorData, TStyle> AsAuthenticationFilterOverrideWhen<TLimit, TActivatorData, TStyle>(this IRegistrationBuilder<TLimit, TActivatorData, TStyle> registration, Func<ControllerContext, ActionDescriptor, bool> filterCondition, FilterScope filterScope, int order)
            where TLimit : IAuthenticationFilter
        {
            return registration.AsFilterWhen<TLimit, TActivatorData, TStyle, IAuthenticationFilter>(filterCondition, filterScope, order, CustomAutofacFilterProvider.AuthenticationFilterOverrideMetadataKey);
        }

        public static IRegistrationBuilder<TLimit, TActivatorData, TStyle> AsAuthorizationFilterOverrideWhen<TLimit, TActivatorData, TStyle>(this IRegistrationBuilder<TLimit, TActivatorData, TStyle> registration, Func<ControllerContext, ActionDescriptor, bool> filterCondition, FilterScope filterScope, int order)
            where TLimit : IAuthorizationFilter
        {
            return registration.AsFilterWhen<TLimit, TActivatorData, TStyle, IAuthorizationFilter>(filterCondition, filterScope, order, CustomAutofacFilterProvider.AuthorizationFilterOverrideMetadataKey);
        }

        public static IRegistrationBuilder<TLimit, TActivatorData, TStyle> AsExceptionFilterOverrideWhen<TLimit, TActivatorData, TStyle>(this IRegistrationBuilder<TLimit, TActivatorData, TStyle> registration, Func<ControllerContext, ActionDescriptor, bool> filterCondition, FilterScope filterScope, int order)
            where TLimit : IExceptionFilter
        {
            return registration.AsFilterWhen<TLimit, TActivatorData, TStyle, IExceptionFilter>(filterCondition, filterScope, order, CustomAutofacFilterProvider.ExceptionFilterOverrideMetadataKey);
        }

        private static IRegistrationBuilder<TLimit, TActivatorData, TStyle> AsFilterWhen<TLimit, TActivatorData, TStyle, TFilterKind>(this IRegistrationBuilder<TLimit, TActivatorData, TStyle> registration, Func<ControllerContext, ActionDescriptor, bool> filterCondition, FilterScope filterScope, int order, string metadataKey)
                    where TLimit : TFilterKind
        {
            var metadata = new CustomFilterMetadata(filterCondition, filterScope, order);
            return registration.As<TFilterKind>().WithMetadata(metadataKey, metadata);
        }
    }
}
