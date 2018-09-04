using System;
using System.Web.Mvc;

namespace FGS.Pump.Extensions.DI.Mvc
{
    internal class AuthorizationFilterReflectiveFacade : IAuthorizationFilter
    {
        private readonly Lazy<IAuthorizationFilter> _lazyAdapted;

        public AuthorizationFilterReflectiveFacade(Lazy<IAuthorizationFilter> lazyAdapted)
        {
            _lazyAdapted = lazyAdapted;
        }

        public void OnAuthorization(AuthorizationContext filterContext) => _lazyAdapted.Value.OnAuthorization(filterContext);

        public override string ToString()
        {
            return _lazyAdapted.Value.ToString();
        }
    }
}