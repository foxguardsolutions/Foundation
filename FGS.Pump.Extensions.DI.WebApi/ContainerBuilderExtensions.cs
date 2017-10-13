using System;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

using Autofac;

namespace FGS.Pump.Extensions.DI.WebApi
{
    public static class ContainerBuilderExtensions
    {
        public static void RegisterWebApiAuthorizationFilterOverrideForWhen(this ContainerBuilder builder, Func<HttpControllerDescriptor, HttpActionDescriptor, bool> filterCondition, FilterScope filterScope, int order)
        {
            RegisterFilterOverrideForWhen<IAuthorizationFilter>(builder, CustomAutofacWebApiFilterProvider.AuthorizationFilterOverrideMetadataKey, filterCondition, filterScope, order);
        }

        public static void RegisterWebApiActionFilterOverrideForWhen(this ContainerBuilder builder, Func<HttpControllerDescriptor, HttpActionDescriptor, bool> filterCondition, FilterScope filterScope, int order)
        {
            RegisterFilterOverrideForWhen<IActionFilter>(builder, CustomAutofacWebApiFilterProvider.ActionFilterOverrideMetadataKey, filterCondition, filterScope, order);
        }

        public static void RegisterWebApiAuthenticationFilterOverrideForWhen(this ContainerBuilder builder, Func<HttpControllerDescriptor, HttpActionDescriptor, bool> filterCondition, FilterScope filterScope, int order)
        {
            RegisterFilterOverrideForWhen<IAuthenticationFilter>(builder, CustomAutofacWebApiFilterProvider.AuthenticationFilterOverrideMetadataKey, filterCondition, filterScope, order);
        }

        public static void RegisterWebApiExceptionFilterOverrideForWhen(this ContainerBuilder builder, Func<HttpControllerDescriptor, HttpActionDescriptor, bool> filterCondition, FilterScope filterScope, int order)
        {
            RegisterFilterOverrideForWhen<IExceptionFilter>(builder, CustomAutofacWebApiFilterProvider.ExceptionFilterOverrideMetadataKey, filterCondition, filterScope, order);
        }

        private static void RegisterFilterOverrideForWhen<TOverriddenFilter>(ContainerBuilder builder, string metadataKey, Func<HttpControllerDescriptor, HttpActionDescriptor, bool> filterCondition, FilterScope filterScope, int order)
        {
            var filterMetadata = new CustomWebApiFilterMetadata()
            {
                Predicate = filterCondition,
                FilterScope = filterScope,
                Order = order
            };
            builder.RegisterInstance(new CustomAutofacOverrideFilter(typeof(TOverriddenFilter))).As<IOverrideFilter>().WithMetadata(metadataKey, filterMetadata);
        }
    }
}
