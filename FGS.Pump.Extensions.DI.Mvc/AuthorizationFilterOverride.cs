using System;
using System.Web.Mvc;
using System.Web.Mvc.Filters;

namespace FGS.Pump.Extensions.DI.Mvc
{
    /// <summary>
    /// Taken and modified from: https://github.com/autofac/Autofac.Mvc/blob/ac6478034bdd32938fdd6b4923519df95f98ab18/src/Autofac.Integration.Mvc/AuthorizationFilterOverride.cs
    /// </summary>
    internal class AuthorizationFilterOverride : IAuthorizationFilter, IOverrideFilter
    {
        private readonly IAuthorizationFilter _wrappedFilter;

        public AuthorizationFilterOverride(IAuthorizationFilter wrappedFilter)
        {
            _wrappedFilter = wrappedFilter;
        }

        public Type FiltersToOverride => typeof(IAuthorizationFilter);

        public void OnAuthorization(AuthorizationContext filterContext) => _wrappedFilter.OnAuthorization(filterContext);
    }
}