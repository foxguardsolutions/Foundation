using System.Web.Mvc;

namespace FGS.Pump.Extensions.DI.Mvc
{
    internal class ActionFilterReflectiveFacade : IActionFilter
    {
        private readonly IActionFilter _adapted;

        public ActionFilterReflectiveFacade(IActionFilter adapted)
        {
            _adapted = adapted;
        }

        public void OnActionExecuting(ActionExecutingContext filterContext) => _adapted.OnActionExecuting(filterContext);

        public void OnActionExecuted(ActionExecutedContext filterContext) => _adapted.OnActionExecuted(filterContext);

        public override string ToString()
        {
            return _adapted.ToString();
        }
    }
}