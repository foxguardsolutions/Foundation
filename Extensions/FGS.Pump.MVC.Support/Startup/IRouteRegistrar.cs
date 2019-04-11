using Microsoft.AspNetCore.Routing;

namespace FGS.Pump.MVC.Support.Startup
{
    public interface IRouteRegistrar
    {
        void RegisterRoutes(IRouteBuilder routes);
    }
}
