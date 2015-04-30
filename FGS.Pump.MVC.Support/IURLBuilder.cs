using System.Web.Mvc;
using System.Web.Routing;

namespace MVCSupport
{
    public interface IURLBuilder
    {
        string GetAction(UrlHelper helper, string action, string controller, RouteValueDictionary dictionary, string protocol);
    }
}
