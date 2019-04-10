using System.Web.Mvc;
using System.Web.Routing;

namespace FGS.Pump.MVC.Support
{
    public interface IURLBuilder
    {
        string GetAction(UrlHelper helper, string action, string controller, RouteValueDictionary dictionary, string protocol);
    }
}
