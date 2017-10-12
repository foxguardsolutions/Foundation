using System.Web.Mvc;

namespace FGS.Pump.Extensions.DI.Mvc
{
    internal class ResultFilterReflectiveFacade : IResultFilter
    {
        private readonly IResultFilter _adapted;

        public ResultFilterReflectiveFacade(IResultFilter adapted)
        {
            _adapted = adapted;
        }

        public void OnResultExecuting(ResultExecutingContext filterContext) => _adapted.OnResultExecuting(filterContext);

        public void OnResultExecuted(ResultExecutedContext filterContext) => _adapted.OnResultExecuted(filterContext);

        public override string ToString()
        {
            return _adapted.ToString();
        }
    }
}