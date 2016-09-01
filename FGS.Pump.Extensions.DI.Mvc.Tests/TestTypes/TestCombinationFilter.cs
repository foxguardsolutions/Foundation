using System.Web.Mvc;
using System.Web.Mvc.Filters;

namespace FGS.Pump.Extensions.DI.Mvc.Tests.TestTypes
{
    /// <remarks>Taken and modified from: https://github.com/autofac/Autofac.Mvc/blob/e26ce3fe9ccc639f1349bcd8aee8e6e4ee066346/test/Autofac.Integration.Mvc.Test/TestTypes.cs </remarks>
    public class TestCombinationFilter : IActionFilter, IAuthenticationFilter, IAuthorizationFilter, IExceptionFilter, IResultFilter
    {
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }

        public void OnAuthorization(AuthorizationContext filterContext)
        {
        }

        public void OnException(ExceptionContext filterContext)
        {
        }

        public void OnResultExecuting(ResultExecutingContext filterContext)
        {
        }

        public void OnResultExecuted(ResultExecutedContext filterContext)
        {
        }

        public void OnAuthentication(AuthenticationContext filterContext)
        {
        }

        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {
        }
    }
}