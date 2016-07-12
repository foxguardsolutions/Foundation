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
    /// <remarks>Taken and modified from: https://github.com/autofac/Autofac.WebApi/blob/7a49de1c8ab3251f364c356e34db4924133542f8/src/Autofac.Integration.WebApi/ExceptionFilterWrapper.cs </remarks>
    [SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes", Justification = "Derived attribute adds filter override support")]
    internal class ExceptionFilterWrapper : ExceptionFilterAttribute, ICustomAutofacExceptionFilter, IFilterWrapper
    {
        private readonly CustomWebApiFilterMetadata _filterMetadata;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionFilterWrapper"/> class.
        /// </summary>
        /// <param name="filterMetadata">The filter metadata.</param>
        public ExceptionFilterWrapper(CustomWebApiFilterMetadata filterMetadata)
        {
            if (filterMetadata == null) throw new ArgumentNullException(nameof(filterMetadata));

            _filterMetadata = filterMetadata;
        }

        /// <summary>
        /// Gets the metadata key used to retrieve the filter metadata.
        /// </summary>
        public virtual string MetadataKey => CustomAutofacWebApiFilterProvider.ExceptionFilterMetadataKey;

        /// <summary>
        /// Called when an exception is thrown.
        /// </summary>
        /// <param name="actionExecutedContext">The context for the action.</param>
        /// /// <param name="cancellationToken">A cancellation token for signaling task ending.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown if <paramref name="actionExecutedContext" /> is <see langword="null" />.
        /// </exception>
        public override async Task OnExceptionAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            if (actionExecutedContext == null) throw new ArgumentNullException(nameof(actionExecutedContext));

            var dependencyScope = actionExecutedContext.Request.GetDependencyScope();
            var lifetimeScope = dependencyScope.GetRequestLifetimeScope();

            var actionDescriptor = actionExecutedContext.ActionContext.ActionDescriptor;
            var filterResoulutionParameters = actionDescriptor.CreateFilterResolutionParameters();
            var metaLazyFilters = lifetimeScope.Resolve<IEnumerable<Meta<Lazy<ICustomAutofacExceptionFilter>>>>(filterResoulutionParameters);

            foreach (var metaLazyFilter in metaLazyFilters.Where(mlf => IsMatchingFilterForThisWrapper(mlf, actionDescriptor)))
                await metaLazyFilter.Value.Value.OnExceptionAsync(actionExecutedContext, cancellationToken);
        }

        private bool IsMatchingFilterForThisWrapper(Meta<Lazy<ICustomAutofacExceptionFilter>> metaLazyFilter, HttpActionDescriptor actionDescriptor)
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
