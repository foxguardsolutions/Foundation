using System;
using System.Web.Http.Filters;

namespace FGS.Pump.Extensions.DI.WebApi
{
    /// <summary>
    /// Allows other filters to be overriden at the control and action level.
    /// </summary>
    /// <remarks>Taken and modified from: https://github.com/autofac/Autofac.WebApi/blob/f764f7e10694a57cf19c968c1ca5b6b998ba82c2/src/Autofac.Integration.WebApi/AutofacOverrideFilter.cs </remarks>
    internal class CustomAutofacOverrideFilter : IOverrideFilter
    {
        public CustomAutofacOverrideFilter(Type filtersToOverride)
        {
            FiltersToOverride = filtersToOverride;
        }

        public bool AllowMultiple => false;

        public Type FiltersToOverride { get; }
    }
}
