using System;
using System.Web.Mvc.Filters;

namespace FGS.Pump.Extensions.DI.Mvc
{
    /// <summary>
    /// Allows other filters to be overriden at the control and action level.
    /// </summary>
    /// <remarks>Taken and modified from: https://github.com/autofac/Autofac.Mvc/blob/ac6478034bdd32938fdd6b4923519df95f98ab18/src/Autofac.Integration.Mvc/AutofacOverrideFilter.cs </remarks>
    internal class CustomAutofacOverrideFilter : IOverrideFilter
    {
        public CustomAutofacOverrideFilter(Type filtersToOverride)
        {
            FiltersToOverride = filtersToOverride;
        }

        public Type FiltersToOverride { get; }
    }
}
