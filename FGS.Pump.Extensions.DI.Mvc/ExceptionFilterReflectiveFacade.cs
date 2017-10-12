using System.Web.Mvc;

namespace FGS.Pump.Extensions.DI.Mvc
{
    internal class ExceptionFilterReflectiveFacade : IExceptionFilter
    {
        private readonly IExceptionFilter _adapted;

        public ExceptionFilterReflectiveFacade(IExceptionFilter adapted)
        {
            _adapted = adapted;
        }

        public void OnException(ExceptionContext filterContext) => _adapted.OnException(filterContext);

        public override string ToString()
        {
            return _adapted.ToString();
        }
    }
}