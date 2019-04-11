using System;

using Microsoft.AspNetCore.Http;

namespace FGS.Pump.MVC.Support
{
    public interface IFormsAuthentication
    {
        [Obsolete("Use ASP.NET Core cookie authentication: https://docs.microsoft.com/en-us/aspnet/core/security/authentication/cookie?view=aspnetcore-2.2")]
        void SetAuthCookie<T>(HttpResponse response, string name, bool rememberMe, T userData);

        [Obsolete("Use ASP.NET Core cookie authentication: https://docs.microsoft.com/en-us/aspnet/core/security/authentication/cookie?view=aspnetcore-2.2")]
        void ClearAuthCookie();

        [Obsolete("Use ASP.NET Core cookie authentication: https://docs.microsoft.com/en-us/aspnet/core/security/authentication/cookie?view=aspnetcore-2.2")]
        object GetFormsAuthenticationTicket(HttpRequest request);
    }
}
