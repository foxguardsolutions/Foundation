using System;
using System.Web.Mvc.Filters;

namespace FGS.Pump.Extensions.DI.Mvc
{
    internal class AuthenticationFilterReflectiveFacade : IAuthenticationFilter
    {
        private readonly Lazy<IAuthenticationFilter> _lazyAdapted;

        public AuthenticationFilterReflectiveFacade(Lazy<IAuthenticationFilter> lazyAdapted)
        {
            _lazyAdapted = lazyAdapted;
        }

        public void OnAuthentication(AuthenticationContext filterContext) => _lazyAdapted.Value.OnAuthentication(filterContext);

        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext) => _lazyAdapted.Value.OnAuthenticationChallenge(filterContext);
        public override string ToString()
        {
            return _lazyAdapted.Value.ToString();
        }
    }
}