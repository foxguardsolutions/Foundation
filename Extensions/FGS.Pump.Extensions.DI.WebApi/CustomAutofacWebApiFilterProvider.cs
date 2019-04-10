using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

using Autofac;
using Autofac.Core.Lifetime;
using Autofac.Features.Metadata;
using Autofac.Integration.WebApi;

namespace FGS.Pump.Extensions.DI.WebApi
{
    /// <summary>
    /// A filter provider for performing property injection on filter attributes.
    /// </summary>
    /// <remarks>Taken and (heavily) modified from: https://github.com/autofac/Autofac.WebApi/blob/7a49de1c8ab3251f364c356e34db4924133542f8/src/Autofac.Integration.WebApi/AutofacWebApiFilterProvider.cs </remarks>
    public class CustomAutofacWebApiFilterProvider : IFilterProvider
    {
        private const string MetadataKeyPrefix = "CustomAutofacWebApi";

        internal const string ActionFilterMetadataKey = MetadataKeyPrefix + "ActionFilter";
        internal const string ActionFilterOverrideMetadataKey = MetadataKeyPrefix + "ActionFilterOverride";
        internal const string AuthorizationFilterMetadataKey = MetadataKeyPrefix + "AuthorizationFilter";
        internal const string AuthorizationFilterOverrideMetadataKey = MetadataKeyPrefix + "AuthorizationFilterOverride";
        internal const string AuthenticationFilterMetadataKey = MetadataKeyPrefix + "AuthenticationFilter";
        internal const string AuthenticationFilterOverrideMetadataKey = MetadataKeyPrefix + "AuthenticationFilterOverride";
        internal const string ExceptionFilterMetadataKey = MetadataKeyPrefix + "ExceptionFilter";
        internal const string ExceptionFilterOverrideMetadataKey = MetadataKeyPrefix + "ExceptionFilterOverride";

        private class FilterContext
        {
            public ILifetimeScope MetadataResolutionLifetimeScope { get; set; }
            public HttpActionDescriptor HttpActionDescriptor { get; set; }
            public List<FilterInfo> AllFilters { get; set; }
            public Dictionary<string, List<CustomWebApiFilterMetadata>> NewlyAddedFilters { get; set; }
        }

        private readonly ILifetimeScope _rootLifetimeScope;
        private readonly ActionDescriptorFilterProvider _filterProvider = new ActionDescriptorFilterProvider();

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomAutofacWebApiFilterProvider"/> class.
        /// </summary>
        public CustomAutofacWebApiFilterProvider(ILifetimeScope lifetimeScope)
        {
            _rootLifetimeScope = lifetimeScope;
        }

        /// <summary>
        /// Returns the collection of filters associated with <paramref name="actionDescriptor"/>.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="actionDescriptor">The action descriptor.</param>
        /// <returns>A collection of filters with instances property injected.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown if <paramref name="configuration" /> is <see langword="null" />.
        /// </exception>
        public IEnumerable<FilterInfo> GetFilters(HttpConfiguration configuration, HttpActionDescriptor actionDescriptor)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            var nativeAttributeFilters = _filterProvider.GetFilters(configuration, actionDescriptor).ToList();

            foreach (var filterInfo in nativeAttributeFilters)
                _rootLifetimeScope.InjectProperties(filterInfo.Instance);

            var descriptor = actionDescriptor as ReflectedHttpActionDescriptor;
            if (descriptor == null) return nativeAttributeFilters;

            // Use a fake scope to resolve the metadata for the filter.
            var rootLifetimeScope = configuration.DependencyResolver.GetRootLifetimeScope();
            using (var metadataResolutionLifetimeScope = rootLifetimeScope.BeginLifetimeScope(MatchingScopeLifetimeTags.RequestLifetimeScopeTag))
            {
                var filterContext = new FilterContext
                {
                    MetadataResolutionLifetimeScope = metadataResolutionLifetimeScope,
                    HttpActionDescriptor = actionDescriptor,
                    AllFilters = nativeAttributeFilters,
                    NewlyAddedFilters = new[]
                    {
                        ActionFilterMetadataKey,
                        ActionFilterOverrideMetadataKey,
                        AuthenticationFilterMetadataKey,
                        AuthenticationFilterOverrideMetadataKey,
                        AuthorizationFilterMetadataKey,
                        AuthorizationFilterOverrideMetadataKey,
                        ExceptionFilterMetadataKey,
                        ExceptionFilterOverrideMetadataKey
                    }.ToDictionary(k => k, _ => new List<CustomWebApiFilterMetadata>())
                };

                // Global scoped override filters (NOOP kind).
                ResolveNoopFilterOverridesInScope(filterContext, FilterScope.Global);

                // Controller scoped override filters (NOOP kind).
                ResolveNoopFilterOverridesInScope(filterContext, FilterScope.Controller);

                // Action scoped override filters (NOOP kind).
                ResolveNoopFilterOverridesInScope(filterContext, FilterScope.Action);

                // Global scoped override filters.
                ResolveFilterOverridesInScope(filterContext, FilterScope.Global);

                // Controller scoped override filters.
                ResolveFilterOverridesInScope(filterContext, FilterScope.Controller);

                // Action scoped override filters.
                ResolveFilterOverridesInScope(filterContext, FilterScope.Action);

                // Global scoped filters.
                ResolveFiltersInScope(filterContext, FilterScope.Global);

                // Controller scoped filters.
                ResolveFiltersInScope(filterContext, FilterScope.Controller);

                // Action scoped filters.
                ResolveFiltersInScope(filterContext, FilterScope.Action);

                return filterContext.AllFilters;
            }
        }

        private static void ResolveNoopFilterOverridesInScope(FilterContext filterContext, FilterScope scope)
        {
            ResolveOverrideFilterInScope(filterContext, AuthenticationFilterOverrideMetadataKey, scope);
            ResolveOverrideFilterInScope(filterContext, AuthorizationFilterOverrideMetadataKey, scope);
            ResolveOverrideFilterInScope(filterContext, ActionFilterOverrideMetadataKey, scope);
            ResolveOverrideFilterInScope(filterContext, ExceptionFilterOverrideMetadataKey, scope);
        }

        private static void ResolveFilterOverridesInScope(FilterContext filterContext, FilterScope scope)
        {
            ResolveFilterInScope<ICustomAutofacAuthenticationFilter, AuthenticationFilterOverrideWrapper>(filterContext, m => new AuthenticationFilterOverrideWrapper(m), AuthenticationFilterOverrideMetadataKey, scope);
            ResolveFilterInScope<ICustomAutofacAuthorizationFilter, AuthorizationFilterOverrideWrapper>(filterContext, m => new AuthorizationFilterOverrideWrapper(m), AuthorizationFilterOverrideMetadataKey, scope);
            ResolveFilterInScope<ICustomAutofacActionFilter, ActionFilterOverrideWrapper>(filterContext, m => new ActionFilterOverrideWrapper(m), ActionFilterOverrideMetadataKey, scope);
            ResolveFilterInScope<ICustomAutofacExceptionFilter, ExceptionFilterOverrideWrapper>(filterContext, m => new ExceptionFilterOverrideWrapper(m), ExceptionFilterOverrideMetadataKey, scope);
        }

        private static void ResolveFiltersInScope(FilterContext filterContext, FilterScope scope)
        {
            ResolveFilterInScope<ICustomAutofacAuthenticationFilter, AuthenticationFilterWrapper>(filterContext, m => new AuthenticationFilterWrapper(m), AuthenticationFilterMetadataKey, scope);
            ResolveFilterInScope<ICustomAutofacAuthorizationFilter, AuthorizationFilterWrapper>(filterContext, m => new AuthorizationFilterWrapper(m), AuthorizationFilterMetadataKey, scope);
            ResolveFilterInScope<ICustomAutofacActionFilter, ActionFilterWrapper>(filterContext, m => new ActionFilterWrapper(m), ActionFilterMetadataKey, scope);
            ResolveFilterInScope<ICustomAutofacExceptionFilter, ExceptionFilterWrapper>(filterContext, m => new ExceptionFilterWrapper(m), ExceptionFilterMetadataKey, scope);
        }

        private static void ResolveFilterInScope<TFilter, TWrapper>(
            FilterContext filterContext, Func<CustomWebApiFilterMetadata, TWrapper> wrapperFactory, string metadataKey, FilterScope scope)
            where TFilter : class
            where TWrapper : IFilter
        {
            var filters = filterContext.MetadataResolutionLifetimeScope.Resolve<IEnumerable<Meta<Lazy<TFilter>>>>();
            filters = filters.Where(f => f.Metadata.ContainsKey(metadataKey) && f.Metadata[metadataKey] is CustomWebApiFilterMetadata);
            var orderedFilters = filters.OrderBy(f => ((CustomWebApiFilterMetadata)f.Metadata[metadataKey]).Order);

            foreach (var filter in orderedFilters)
            {
                var metadata = (CustomWebApiFilterMetadata)filter.Metadata[metadataKey];

                if (!FilterMatchesInScope(filterContext, metadataKey, metadata, scope)) continue;

                var wrapper = wrapperFactory(metadata);
                filterContext.AllFilters.Add(new FilterInfo(wrapper, metadata.FilterScope));
                filterContext.NewlyAddedFilters[metadataKey].Add(metadata);
            }
        }

        private static void ResolveOverrideFilterInScope(FilterContext filterContext, string metadataKey, FilterScope scope)
        {
            var filters = filterContext.MetadataResolutionLifetimeScope.Resolve<IEnumerable<Meta<IOverrideFilter>>>();
            filters = filters.Where(f => f.Metadata.ContainsKey(metadataKey) && f.Metadata[metadataKey] is CustomWebApiFilterMetadata);
            var orderedFilters = filters.OrderBy(f => ((CustomWebApiFilterMetadata)f.Metadata[metadataKey]).Order);

            foreach (var filter in orderedFilters)
            {
                var metadata = (CustomWebApiFilterMetadata)filter.Metadata[metadataKey];

                if (!FilterMatchesInScope(filterContext, metadataKey, metadata, scope)) continue;

                filterContext.AllFilters.Add(new FilterInfo(filter.Value, metadata.FilterScope));
                filterContext.NewlyAddedFilters[metadataKey].Add(metadata);
            }
        }

        private static bool FilterMatchesInScope(FilterContext filterContext, string metadataKey, CustomWebApiFilterMetadata metadata, FilterScope scope)
        {
            return metadata.FilterScope == scope
                   && metadata.Predicate(filterContext.HttpActionDescriptor.ControllerDescriptor, filterContext.HttpActionDescriptor)
                   && !filterContext.NewlyAddedFilters[metadataKey].Contains(metadata);
        }
    }
}
