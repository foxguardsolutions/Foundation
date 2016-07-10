using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Mvc.Filters;

using Autofac;
using Autofac.Core;
using Autofac.Features.Metadata;
using Autofac.Integration.Mvc;

namespace FGS.Pump.Extensions.DI.Mvc
{
    /// <summary>
    /// Defines a filter provider that uses metadata conditions to resolve filters
    /// </summary>
    /// <remarks>Taken and (heavily) modified from: https://github.com/autofac/Autofac.Mvc/blob/ac6478034bdd32938fdd6b4923519df95f98ab18/src/Autofac.Integration.Mvc/AutofacFilterProvider.cs </remarks>
    public class CustomAutofacFilterProvider : FilterAttributeFilterProvider
    {
        private const string CommonKeyPrefix = "CustomAutofacMvc";

        internal const string ActionFilterMetadataKey = CommonKeyPrefix + "ActionFilter";
        internal const string ActionFilterOverrideMetadataKey = CommonKeyPrefix + "ActionFilterOverride";
        internal const string AuthenticationFilterMetadataKey = CommonKeyPrefix + "AuthenticationFilter";
        internal const string AuthenticationFilterOverrideMetadataKey = CommonKeyPrefix + "AuthenticationFilterOverride";
        internal const string AuthorizationFilterMetadataKey = CommonKeyPrefix + "AuthorizationFilter";
        internal const string AuthorizationFilterOverrideMetadataKey = CommonKeyPrefix + "AuthorizationFilterOverride";
        internal const string ExceptionFilterMetadataKey = CommonKeyPrefix + "ExceptionFilter";
        internal const string ExceptionFilterOverrideMetadataKey = CommonKeyPrefix + "ExceptionFilterOverride";
        internal const string ResultFilterMetadataKey = CommonKeyPrefix + "ResultFilter";
        internal const string ResultFilterOverrideMetadataKey = CommonKeyPrefix + "ResultFilterOverride";
        internal const string ControllerContextParameterName = CommonKeyPrefix + "ControllerContext";
        internal const string ActionDescriptorParameterName = CommonKeyPrefix + "ActionDescriptor";

        /// <summary>
        /// Initializes a new instance of the <see cref="AutofacFilterProvider"/> class.
        /// </summary>
        /// <remarks>
        /// The <c>false</c> constructor parameter passed to base here ensures that attribute instances are not cached.
        /// </remarks>
        public CustomAutofacFilterProvider()
            : base(false)
        {
        }

        /// <summary>
        /// Aggregates the filters from all of the filter providers into one collection.
        /// </summary>
        /// <param name="controllerContext">The controller context.</param>
        /// <param name="actionDescriptor">The action descriptor.</param>
        /// <returns>
        /// The collection filters from all of the filter providers with properties injected.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown if <paramref name="controllerContext" /> is <see langword="null" />.
        /// </exception>
        public override IEnumerable<Filter> GetFilters(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            if (controllerContext == null) throw new ArgumentNullException(nameof(controllerContext));

            var filters = base.GetFilters(controllerContext, actionDescriptor).ToList();
            var lifetimeScope = AutofacDependencyResolver.Current.RequestLifetimeScope;

            if (lifetimeScope != null)
            {
                foreach (var filter in filters)
                {
                    lifetimeScope.InjectProperties(filter.Instance);
                }

                var filterContext = new FilterContext
                {
                    ControllerContext = controllerContext,
                    ActionDescriptor = actionDescriptor,
                    LifetimeScope = lifetimeScope,
                    Filters = filters
                };

                ResolveControllerScopedFilters(filterContext);

                ResolveActionScopedFilters(filterContext);

                ResolveControllerScopedFilterOverrides(filterContext);

                ResolveActionScopedFilterOverrides(filterContext);

                ResolveControllerScopedEmptyOverrideFilters(filterContext);

                ResolveActionScopedEmptyOverrideFilters(filterContext);
            }

            return filters.ToArray();
        }

        private static bool FilterMatchesAction(FilterContext filterContext, CustomFilterMetadata metadata)
        {
            return metadata.FilterScope == FilterScope.Action
                   && metadata.ControllerPredicate(filterContext.ControllerContext)
                   && metadata.ActionPredicate(filterContext.ControllerContext, filterContext.ActionDescriptor);
        }

        private static bool FilterMatchesController(FilterContext filterContext, CustomFilterMetadata metadata)
        {
            return metadata.FilterScope == FilterScope.Controller
                   && metadata.ControllerPredicate(filterContext.ControllerContext)
                   && metadata.ActionPredicate(filterContext.ControllerContext, filterContext.ActionDescriptor);
        }

        private static void ResolveActionScopedEmptyOverrideFilters(FilterContext filterContext)
        {
            ResolveActionScopedOverrideFilter(filterContext, ActionFilterOverrideMetadataKey);
            ResolveActionScopedOverrideFilter(filterContext, AuthenticationFilterOverrideMetadataKey);
            ResolveActionScopedOverrideFilter(filterContext, AuthorizationFilterOverrideMetadataKey);
            ResolveActionScopedOverrideFilter(filterContext, ExceptionFilterOverrideMetadataKey);
            ResolveActionScopedOverrideFilter(filterContext, ResultFilterOverrideMetadataKey);
        }

        private static void ResolveActionScopedFilter<TFilter>(FilterContext filterContext, string metadataKey, Func<TFilter, TFilter> wrapperFactory = null)
            where TFilter : class
        {
            var resolveParameters = CreateResolveParameters(filterContext);
            var actionFilters = filterContext.LifetimeScope.Resolve<IEnumerable<Meta<Lazy<TFilter>>>>(resolveParameters);

            foreach (var actionFilter in actionFilters.Where(ContainsCustomMetadataForKey<Lazy<TFilter>>(metadataKey)))
            {
                var metadata = (CustomFilterMetadata)actionFilter.Metadata[metadataKey];
                if (!FilterMatchesAction(filterContext, metadata))
                {
                    continue;
                }

                var instance = actionFilter.Value.Value;

                if (wrapperFactory != null)
                {
                    instance = wrapperFactory(instance);
                }

                var filter = new Filter(instance, FilterScope.Action, metadata.Order);
                filterContext.Filters.Add(filter);
            }
        }

        private static void ResolveActionScopedFilterOverrides(FilterContext filterContext)
        {
            ResolveActionScopedFilter<IActionFilter>(filterContext, ActionFilterOverrideMetadataKey, filter => new ActionFilterOverride(filter));
            ResolveActionScopedFilter<IAuthenticationFilter>(filterContext, AuthenticationFilterOverrideMetadataKey, filter => new AuthenticationFilterOverride(filter));
            ResolveActionScopedFilter<IAuthorizationFilter>(filterContext, AuthorizationFilterOverrideMetadataKey, filter => new AuthorizationFilterOverride(filter));
            ResolveActionScopedFilter<IExceptionFilter>(filterContext, ExceptionFilterOverrideMetadataKey, filter => new ExceptionFilterOverride(filter));
            ResolveActionScopedFilter<IResultFilter>(filterContext, ResultFilterOverrideMetadataKey, filter => new ResultFilterOverride(filter));
        }

        private static void ResolveActionScopedFilters(FilterContext filterContext)
        {
            ResolveActionScopedFilter<IActionFilter>(filterContext, ActionFilterMetadataKey);
            ResolveActionScopedFilter<IAuthenticationFilter>(filterContext, AuthenticationFilterMetadataKey);
            ResolveActionScopedFilter<IAuthorizationFilter>(filterContext, AuthorizationFilterMetadataKey);
            ResolveActionScopedFilter<IExceptionFilter>(filterContext, ExceptionFilterMetadataKey);
            ResolveActionScopedFilter<IResultFilter>(filterContext, ResultFilterMetadataKey);
        }

        private static void ResolveActionScopedOverrideFilter(FilterContext filterContext, string metadataKey)
        {
            var resolveParameters = CreateResolveParameters(filterContext);
            var actionFilters = filterContext.LifetimeScope.Resolve<IEnumerable<Meta<IOverrideFilter>>>(resolveParameters);

            foreach (var actionFilter in actionFilters.Where(ContainsCustomMetadataForKey<IOverrideFilter>(metadataKey)))
            {
                var metadata = (CustomFilterMetadata)actionFilter.Metadata[metadataKey];
                if (!FilterMatchesAction(filterContext, metadata))
                {
                    continue;
                }

                var filter = new Filter(actionFilter.Value, FilterScope.Action, metadata.Order);
                filterContext.Filters.Add(filter);
            }
        }

        private static void ResolveControllerScopedEmptyOverrideFilters(FilterContext filterContext)
        {
            ResolveControllerScopedOverrideFilter(filterContext, ActionFilterOverrideMetadataKey);
            ResolveControllerScopedOverrideFilter(filterContext, AuthenticationFilterOverrideMetadataKey);
            ResolveControllerScopedOverrideFilter(filterContext, AuthorizationFilterOverrideMetadataKey);
            ResolveControllerScopedOverrideFilter(filterContext, ExceptionFilterOverrideMetadataKey);
            ResolveControllerScopedOverrideFilter(filterContext, ResultFilterOverrideMetadataKey);
        }

        private static void ResolveControllerScopedFilter<TFilter>(FilterContext filterContext, string metadataKey, Func<TFilter, TFilter> wrapperFactory = null)
            where TFilter : class
        {
            var resolveParameters = CreateResolveParameters(filterContext);
            var actionFilters = filterContext.LifetimeScope.Resolve<IEnumerable<Meta<Lazy<TFilter>>>>(resolveParameters);

            foreach (var actionFilter in actionFilters.Where(ContainsCustomMetadataForKey<Lazy<TFilter>>(metadataKey)))
            {
                var metadata = (CustomFilterMetadata)actionFilter.Metadata[metadataKey];
                if (!FilterMatchesController(filterContext, metadata))
                {
                    continue;
                }

                var instance = actionFilter.Value.Value;

                if (wrapperFactory != null)
                {
                    instance = wrapperFactory(instance);
                }

                var filter = new Filter(instance, FilterScope.Controller, metadata.Order);
                filterContext.Filters.Add(filter);
            }
        }

        private static void ResolveControllerScopedFilterOverrides(FilterContext filterContext)
        {
            ResolveControllerScopedFilter<IActionFilter>(filterContext, ActionFilterOverrideMetadataKey, filter => new ActionFilterOverride(filter));
            ResolveControllerScopedFilter<IAuthenticationFilter>(filterContext, AuthenticationFilterOverrideMetadataKey, filter => new AuthenticationFilterOverride(filter));
            ResolveControllerScopedFilter<IAuthorizationFilter>(filterContext, AuthorizationFilterOverrideMetadataKey, filter => new AuthorizationFilterOverride(filter));
            ResolveControllerScopedFilter<IExceptionFilter>(filterContext, ExceptionFilterOverrideMetadataKey, filter => new ExceptionFilterOverride(filter));
            ResolveControllerScopedFilter<IResultFilter>(filterContext, ResultFilterOverrideMetadataKey, filter => new ResultFilterOverride(filter));
        }

        private static void ResolveControllerScopedFilters(FilterContext filterContext)
        {
            ResolveControllerScopedFilter<IActionFilter>(filterContext, ActionFilterMetadataKey);
            ResolveControllerScopedFilter<IAuthenticationFilter>(filterContext, AuthenticationFilterMetadataKey);
            ResolveControllerScopedFilter<IAuthorizationFilter>(filterContext, AuthorizationFilterMetadataKey);
            ResolveControllerScopedFilter<IExceptionFilter>(filterContext, ExceptionFilterMetadataKey);
            ResolveControllerScopedFilter<IResultFilter>(filterContext, ResultFilterMetadataKey);
        }

        private static void ResolveControllerScopedOverrideFilter(FilterContext filterContext, string metadataKey)
        {
            var resolveParameters = CreateResolveParameters(filterContext);
            var actionFilters = filterContext.LifetimeScope.Resolve<IEnumerable<Meta<IOverrideFilter>>>(resolveParameters);

            foreach (var actionFilter in actionFilters.Where(ContainsCustomMetadataForKey<IOverrideFilter>(metadataKey)))
            {
                var metadata = (CustomFilterMetadata)actionFilter.Metadata[metadataKey];
                if (!FilterMatchesController(filterContext, metadata))
                {
                    continue;
                }

                var filter = new Filter(actionFilter.Value, FilterScope.Controller, metadata.Order);
                filterContext.Filters.Add(filter);
            }
        }

        private static Func<Meta<T>, bool> ContainsCustomMetadataForKey<T>(string metadataKey)
        {
            return a => a.Metadata.ContainsKey(metadataKey) && a.Metadata[metadataKey] is CustomFilterMetadata;
        }

        private static Parameter[] CreateResolveParameters(FilterContext filterContext)
        {
            var resolveParameters = new Parameter[]
                                        {
                                            new NamedParameter(ControllerContextParameterName, filterContext.ControllerContext),
                                            new NamedParameter(ActionDescriptorParameterName, filterContext.ActionDescriptor)
                                        };
            return resolveParameters;
        }

        private class FilterContext
        {
            public ControllerContext ControllerContext { get; set; }

            public ActionDescriptor ActionDescriptor { get; set; }

            public List<Filter> Filters { get; set; }

            public ILifetimeScope LifetimeScope { get; set; }
        }
    }
}
