using System.Web.Http;

namespace FGS.Pump.MVC.Support.Startup
{
    public interface IWebApiConfigurator
    {
        void Register(HttpConfiguration config);
    }
}
