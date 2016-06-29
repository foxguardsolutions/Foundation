using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace FGS.Pump.Extensions.DI.WebApi
{
    public class ApiActionFilterDecoraptor<T> : IActionFilter
        where T : IActionFilter
    {
        private readonly Func<T> _filterFactory;
        private readonly Lazy<bool> _lazyAllowMultiple;

        public ApiActionFilterDecoraptor(Func<T> filterFactory)
        {
            _filterFactory = filterFactory;
            _lazyAllowMultiple = new Lazy<bool>(filterFactory().AllowMultiple);
        }

        public bool AllowMultiple => _lazyAllowMultiple.Value;

        public Task<HttpResponseMessage> ExecuteActionFilterAsync(HttpActionContext actionContext, CancellationToken cancellationToken, Func<Task<HttpResponseMessage>> continuation)
        {
            return _filterFactory().ExecuteActionFilterAsync(actionContext, cancellationToken, continuation);
        }
    }
}