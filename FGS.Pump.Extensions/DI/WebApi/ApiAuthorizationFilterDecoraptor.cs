using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace FGS.Pump.Extensions.DI.WebApi
{
    public class ApiAuthorizationFilterDecoraptor<T> : IAuthorizationFilter
        where T : IAuthorizationFilter
    {
        private readonly Func<T> _filterFactory;
        private readonly Lazy<bool> _lazyAllowMultiple;

        public ApiAuthorizationFilterDecoraptor(Func<T> filterFactory)
        {
            _filterFactory = filterFactory;
            _lazyAllowMultiple = new Lazy<bool>(filterFactory().AllowMultiple);
        }

        public bool AllowMultiple => _lazyAllowMultiple.Value;

        public Task<HttpResponseMessage> ExecuteAuthorizationFilterAsync(
            HttpActionContext actionContext,
            CancellationToken cancellationToken,
            Func<Task<HttpResponseMessage>> continuation)
        {
            return _filterFactory().ExecuteAuthorizationFilterAsync(actionContext, cancellationToken, continuation);
        }
    }
}