using System.Web;
using System.Web.Security;

namespace MVCSupport
{
    public interface IFormsAuthentication
    {
        void SetAuthCookie<T>(HttpResponseBase response, string name, bool rememberMe, T userData);
        void ClearAuthCookie();
        FormsAuthenticationTicket GetFormsAuthenticationTicket(HttpRequestBase request);

    }
}
