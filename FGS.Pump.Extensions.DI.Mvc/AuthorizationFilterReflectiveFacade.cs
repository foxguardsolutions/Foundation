using System.Web.Mvc;

namespace FGS.Pump.Extensions.DI.Mvc
{
    internal class AuthorizationFilterReflectiveFacade : IAuthorizationFilter
    {
        private readonly IAuthorizationFilter _adapted;

        public AuthorizationFilterReflectiveFacade(IAuthorizationFilter adapted)
        {
            _adapted = adapted;
        }

        public void OnAuthorization(AuthorizationContext filterContext) => _adapted.OnAuthorization(filterContext);

        public override string ToString()
        {
            return _adapted.ToString();
        }
    }
}