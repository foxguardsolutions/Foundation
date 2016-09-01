using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

using Autofac;
using Autofac.Features.Metadata;
using Autofac.Integration.WebApi;

namespace FGS.Pump.Extensions.DI.WebApi
{
    /// <summary>
    /// Resolves a filter for the specified metadata for each controller request.
    /// </summary>
    /// <remarks>Taken and modified from: https://github.com/autofac/Autofac.WebApi/blob/7a49de1c8ab3251f364c356e34db4924133542f8/src/Autofac.Integration.WebApi/AuthenticationFilterWrapper.cs </remarks>
    internal class AuthenticationFilterWrapper : IAuthenticationFilter, ICustomAutofacAuthenticationFilter, IFilterWrapper
    {
        private readonly CustomWebApiFilterMetadata _filterMetadata;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationFilterWrapper"/> class.
        /// </summary>
        /// <param name="filterMetadata">The filter metadata.</param>
        public AuthenticationFilterWrapper(CustomWebApiFilterMetadata filterMetadata)
        {
            if (filterMetadata == null) throw new ArgumentNullException(nameof(filterMetadata));

            _filterMetadata = filterMetadata;
        }

        /// <summary>
        /// Gets the metadata key used to retrieve the filter metadata.
        /// </summary>
        public virtual string MetadataKey => CustomAutofacWebApiFilterProvider.AuthenticationFilterMetadataKey;

        public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var dependencyScope = context.Request.GetDependencyScope();
            var lifetimeScope = dependencyScope.GetRequestLifetimeScope();

            var actionDescriptor = context.ActionContext.ActionDescriptor;
            var filterResoulutionParameters = actionDescriptor.CreateFilterResolutionParameters();
            var metaLazyFilters = lifetimeScope.Resolve<IEnumerable<Meta<Lazy<ICustomAutofacAuthenticationFilter>>>>(filterResoulutionParameters);

            foreach (var metaLazyFilter in metaLazyFilters.Where(mlf => IsMatchingFilterForThisWrapper(mlf, actionDescriptor)))
                await metaLazyFilter.Value.Value.AuthenticateAsync(context, cancellationToken);
        }

        public async Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var dependencyScope = context.Request.GetDependencyScope();
            var lifetimeScope = dependencyScope.GetRequestLifetimeScope();

            var actionDescriptor = context.ActionContext.ActionDescriptor;
            var filterResoulutionParameters = actionDescriptor.CreateFilterResolutionParameters();
            var metaLazyFilters = lifetimeScope.Resolve<IEnumerable<Meta<Lazy<ICustomAutofacAuthenticationFilter>>>>(filterResoulutionParameters);

            foreach (var metaLazyFilter in metaLazyFilters.Where(mlf => IsMatchingFilterForThisWrapper(mlf, actionDescriptor)))
                await metaLazyFilter.Value.Value.ChallengeAsync(context, cancellationToken);
        }

        bool IFilter.AllowMultiple => true;

        private bool IsMatchingFilterForThisWrapper(Meta<Lazy<ICustomAutofacAuthenticationFilter>> metaLazyFilter, HttpActionDescriptor actionDescriptor)
        {
            var metadata = metaLazyFilter.Metadata.ContainsKey(MetadataKey)
                ? metaLazyFilter.Metadata[MetadataKey] as CustomWebApiFilterMetadata : null;

            return metadata != null
                   && metadata.FilterScope == _filterMetadata.FilterScope
                   && metadata.Predicate(actionDescriptor.ControllerDescriptor, actionDescriptor)
                   && _filterMetadata.Predicate(actionDescriptor.ControllerDescriptor, actionDescriptor);
        }
    }
}
