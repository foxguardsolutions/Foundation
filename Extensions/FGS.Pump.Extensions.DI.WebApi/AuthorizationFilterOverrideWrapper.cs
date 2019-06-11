﻿using System;
using System.Web.Http.Filters;

namespace FGS.Pump.Extensions.DI.WebApi
{
    /// <summary>
    /// Resolves a filter override for the specified metadata for each controller request.
    /// </summary>
    /// <remarks>Taken and modified from: https://github.com/autofac/Autofac.WebApi/blob/7a49de1c8ab3251f364c356e34db4924133542f8/src/Autofac.Integration.WebApi/AuthorizationFilterOverrideWrapper.cs </remarks>
    internal sealed class AuthorizationFilterOverrideWrapper : AuthorizationFilterWrapper, IOverrideFilter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizationFilterOverrideWrapper"/> class.
        /// </summary>
        /// <param name="filterMetadata">The filter metadata.</param>
        public AuthorizationFilterOverrideWrapper(CustomWebApiFilterMetadata filterMetadata)
            : base(filterMetadata)
        {
        }

        /// <summary>
        /// Gets the metadata key used to retrieve the filter metadata.
        /// </summary>
        public override string MetadataKey => CustomAutofacWebApiFilterProvider.AuthorizationFilterOverrideMetadataKey;

        /// <summary>
        /// Gets the filters to override.
        /// </summary>
        public Type FiltersToOverride => typeof(IAuthorizationFilter);
    }
}