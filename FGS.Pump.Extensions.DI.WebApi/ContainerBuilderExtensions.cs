using System;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

using Autofac;

namespace FGS.Pump.Extensions.DI.WebApi
{
    public static class ContainerBuilderExtensions
    {
        public static void RegisterWebApiAuthorizationFilterOverrideForWhen(this ContainerBuilder builder, Func<HttpControllerDescriptor, HttpActionDescriptor, bool> filterCondition, FilterScope filterScope)
        {
            RegisterFilterOverrideForWhen<IAuthorizationFilter>(builder, CustomAutofacWebApiFilterProvider.AuthorizationFilterOverrideMetadataKey, filterCondition, filterScope);
        }

        public static void RegisterWebApiActionFilterOverrideForWhen(this ContainerBuilder builder, Func<HttpControllerDescriptor, HttpActionDescriptor, bool> filterCondition, FilterScope filterScope)
        {
            RegisterFilterOverrideForWhen<IActionFilter>(builder, CustomAutofacWebApiFilterProvider.ActionFilterOverrideMetadataKey, filterCondition, filterScope);
        }

        public static void RegisterWebApiAuthenticationFilterOverrideForWhen(this ContainerBuilder builder, Func<HttpControllerDescriptor, HttpActionDescriptor, bool> filterCondition, FilterScope filterScope)
        {
            RegisterFilterOverrideForWhen<IAuthenticationFilter>(builder, CustomAutofacWebApiFilterProvider.AuthenticationFilterOverrideMetadataKey, filterCondition, filterScope);
        }

        public static void RegisterWebApiExceptionFilterOverrideForWhen(this ContainerBuilder builder, Func<HttpControllerDescriptor, HttpActionDescriptor, bool> filterCondition, FilterScope filterScope)
        {
            RegisterFilterOverrideForWhen<IExceptionFilter>(builder, CustomAutofacWebApiFilterProvider.ExceptionFilterOverrideMetadataKey, filterCondition, filterScope);
        }

        private static void RegisterFilterOverrideForWhen<TOverriddenFilter>(ContainerBuilder builder, string metadataKey, Func<HttpControllerDescriptor, HttpActionDescriptor, bool> filterCondition, FilterScope filterScope)
        {
            var filterMetadata = new CustomWebApiFilterMetadata()
            {
                Predicate = filterCondition,
                FilterScope = filterScope
            };
            builder.RegisterInstance(new CustomAutofacOverrideFilter(typeof(TOverriddenFilter))).As<IOverrideFilter>().WithMetadata(metadataKey, filterMetadata);
        }
    }
}
