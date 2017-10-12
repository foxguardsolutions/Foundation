using System.Web.Mvc.Filters;

namespace FGS.Pump.Extensions.DI.Mvc
{
    internal class AuthenticationFilterReflectiveFacade : IAuthenticationFilter
    {
        private readonly IAuthenticationFilter _adapted;

        public AuthenticationFilterReflectiveFacade(IAuthenticationFilter adapted)
        {
            _adapted = adapted;
        }

        public void OnAuthentication(AuthenticationContext filterContext) => _adapted.OnAuthentication(filterContext);

        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext) => _adapted.OnAuthenticationChallenge(filterContext);
        public override string ToString()
        {
            return _adapted.ToString();
        }
    }
}