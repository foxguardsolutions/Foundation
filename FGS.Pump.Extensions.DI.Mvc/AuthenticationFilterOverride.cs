using System;
using System.Web.Mvc.Filters;

namespace FGS.Pump.Extensions.DI.Mvc
{
    /// <remarks>Taken and modified from: https://github.com/autofac/Autofac.Mvc/blob/ac6478034bdd32938fdd6b4923519df95f98ab18/src/Autofac.Integration.Mvc/AuthenticationFilterOverride.cs </remarks>
    internal class AuthenticationFilterOverride : IAuthenticationFilter, IOverrideFilter
    {
        private readonly IAuthenticationFilter _wrappedFilter;

        public AuthenticationFilterOverride(IAuthenticationFilter wrappedFilter)
        {
            _wrappedFilter = wrappedFilter;
        }

        public Type FiltersToOverride => typeof(IAuthenticationFilter);

        public void OnAuthentication(AuthenticationContext filterContext) => _wrappedFilter.OnAuthentication(filterContext);

        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext) => _wrappedFilter.OnAuthenticationChallenge(filterContext);
    }
}