using System;
using System.Web.Mvc;
using System.Web.Mvc.Filters;

namespace FGS.Pump.Extensions.DI.Mvc
{
    /// <remarks>Taken and modified from: https://github.com/autofac/Autofac.Mvc/blob/ac6478034bdd32938fdd6b4923519df95f98ab18/src/Autofac.Integration.Mvc/ResultFilterOverride.cs </remarks>
    internal class ResultFilterOverride : IResultFilter, IOverrideFilter
    {
        private readonly IResultFilter _wrappedFilter;

        public ResultFilterOverride(IResultFilter wrappedFilter)
        {
            _wrappedFilter = wrappedFilter;
        }

        public Type FiltersToOverride => typeof(IResultFilter);

        public void OnResultExecuted(ResultExecutedContext filterContext) => _wrappedFilter.OnResultExecuted(filterContext);

        public void OnResultExecuting(ResultExecutingContext filterContext) => _wrappedFilter.OnResultExecuting(filterContext);
    }
}