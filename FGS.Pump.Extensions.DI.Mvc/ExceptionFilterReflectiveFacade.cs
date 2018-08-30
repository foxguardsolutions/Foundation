using System;
using System.Web.Mvc;

namespace FGS.Pump.Extensions.DI.Mvc
{
    internal class ExceptionFilterReflectiveFacade : IExceptionFilter
    {
        private readonly Lazy<IExceptionFilter> _lazyAdapted;

        public ExceptionFilterReflectiveFacade(Lazy<IExceptionFilter> lazyAdapted)
        {
            _lazyAdapted = lazyAdapted;
        }

        public void OnException(ExceptionContext filterContext) => _lazyAdapted.Value.OnException(filterContext);

        public override string ToString()
        {
            return _lazyAdapted.Value.ToString();
        }
    }
}