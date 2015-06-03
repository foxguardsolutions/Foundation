using System.Collections.Generic;
using System.Web.Routing;

namespace FGS.Pump.MVC.Support.Startup
{
    public class CompositeRouteRegistrar : IRouteRegistrar
    {
        private readonly IEnumerable<IRouteRegistrar> _routeRegistrars;

        public CompositeRouteRegistrar(IEnumerable<IRouteRegistrar> routeRegistrars)
        {
            _routeRegistrars = routeRegistrars;
        }

        #region Implementation of IRouteRegistrar

        public void RegisterRoutes(RouteCollection routes)
        {
            foreach (var routeRegistrar in _routeRegistrars)
                routeRegistrar.RegisterRoutes(routes);
        }

        #endregion
    }
}