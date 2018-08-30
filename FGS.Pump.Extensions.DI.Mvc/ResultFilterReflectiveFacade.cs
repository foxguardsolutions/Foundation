using System;
using System.Web.Mvc;

namespace FGS.Pump.Extensions.DI.Mvc
{
    internal class ResultFilterReflectiveFacade : IResultFilter
    {
        private readonly Lazy<IResultFilter> _lazyAdapted;

        public ResultFilterReflectiveFacade(Lazy<IResultFilter> lazyAdapted)
        {
            _lazyAdapted = lazyAdapted;
        }

        public void OnResultExecuting(ResultExecutingContext filterContext) => _lazyAdapted.Value.OnResultExecuting(filterContext);

        public void OnResultExecuted(ResultExecutedContext filterContext) => _lazyAdapted.Value.OnResultExecuted(filterContext);

        public override string ToString()
        {
            return _lazyAdapted.Value.ToString();
        }
    }
}