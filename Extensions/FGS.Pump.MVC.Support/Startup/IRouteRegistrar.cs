using System.Web.Routing;

namespace FGS.Pump.MVC.Support.Startup
{
    public interface IRouteRegistrar
    {
        void RegisterRoutes(RouteCollection routes);
    }
}
