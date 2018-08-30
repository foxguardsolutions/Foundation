using System;
using System.Web.Mvc;

namespace FGS.Pump.Extensions.DI.Mvc
{
    internal class ActionFilterReflectiveFacade : IActionFilter
    {
        private readonly Lazy<IActionFilter> _lazyAdapted;

        public ActionFilterReflectiveFacade(Lazy<IActionFilter> lazyAdapted)
        {
            _lazyAdapted = lazyAdapted;
        }

        public void OnActionExecuting(ActionExecutingContext filterContext) => _lazyAdapted.Value.OnActionExecuting(filterContext);

        public void OnActionExecuted(ActionExecutedContext filterContext) => _lazyAdapted.Value.OnActionExecuted(filterContext);

        public override string ToString()
        {
            return _lazyAdapted.Value.ToString();
        }
    }
}