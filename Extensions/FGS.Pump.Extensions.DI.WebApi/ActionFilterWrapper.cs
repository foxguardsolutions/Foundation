using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
    /// <remarks>Taken and modified from: https://github.com/autofac/Autofac.WebApi/blob/7a49de1c8ab3251f364c356e34db4924133542f8/src/Autofac.Integration.WebApi/ActionFilterWrapper.cs </remarks>
    [SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes", Justification = "Derived attribute adds filter override support")]
    internal class ActionFilterWrapper : ActionFilterAttribute, ICustomAutofacActionFilter, IFilterWrapper
    {
        private readonly CustomWebApiFilterMetadata _filterMetadata;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionFilterWrapper"/> class.
        /// </summary>
        /// <param name="filterMetadata">The filter metadata.</param>
        public ActionFilterWrapper(CustomWebApiFilterMetadata filterMetadata)
        {
            _filterMetadata = filterMetadata ?? throw new ArgumentNullException(nameof(filterMetadata));
        }

        /// <summary>
        /// Gets the metadata key used to retrieve the filter metadata.
        /// </summary>
        public virtual string MetadataKey => CustomAutofacWebApiFilterProvider.ActionFilterMetadataKey;

        /// <summary>
        /// Occurs before the action method is invoked.
        /// </summary>
        /// <param name="actionContext">The context for the action.</param>
        /// <param name="cancellationToken">A cancellation token for signaling task ending.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown if <paramref name="actionContext" /> is <see langword="null" />.
        /// </exception>
        public override async Task OnActionExecutingAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            if (actionContext == null) throw new ArgumentNullException(nameof(actionContext));

            var dependencyScope = actionContext.Request.GetDependencyScope();
            var lifetimeScope = dependencyScope.GetRequestLifetimeScope();

            var metaLazyFilters = lifetimeScope.Resolve<IEnumerable<Meta<Lazy<ICustomAutofacActionFilter>>>>();

            var actionDescriptor = actionContext.ActionDescriptor;
            foreach (var metaLazyFilter in metaLazyFilters.Where(mlf => IsMatchingFilterForThisWrapper(mlf, actionDescriptor)))
                await metaLazyFilter.Value.Value.OnActionExecutingAsync(actionContext, cancellationToken);
        }

        /// <summary>
        /// Occurs after the action method is invoked.
        /// </summary>
        /// <param name="actionExecutedContext">The context for the action.</param>
        /// <param name="cancellationToken">A cancellation token for signaling task ending.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown if <paramref name="actionExecutedContext" /> is <see langword="null" />.
        /// </exception>
        public override async Task OnActionExecutedAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            if (actionExecutedContext == null) throw new ArgumentNullException(nameof(actionExecutedContext));

            var dependencyScope = actionExecutedContext.Request.GetDependencyScope();
            var lifetimeScope = dependencyScope.GetRequestLifetimeScope();

            var actionDescriptor = actionExecutedContext.ActionContext.ActionDescriptor;
            var filterResoulutionParameters = actionDescriptor.CreateFilterResolutionParameters();
            var metaLazyFilters = lifetimeScope.Resolve<IEnumerable<Meta<Lazy<ICustomAutofacActionFilter>>>>(filterResoulutionParameters);

            foreach (var metaLazyFilter in metaLazyFilters.Where(mlf => IsMatchingFilterForThisWrapper(mlf, actionDescriptor)))
                await metaLazyFilter.Value.Value.OnActionExecutedAsync(actionExecutedContext, cancellationToken);
        }

        private bool IsMatchingFilterForThisWrapper(Meta<Lazy<ICustomAutofacActionFilter>> metaLazyFilter, HttpActionDescriptor actionDescriptor)
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
