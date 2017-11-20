using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;

namespace FGS.Pump.Extensions.DI.WebApi.Tests
{
    public class TestAuthorizationFilter2 : ICustomAutofacAuthorizationFilter
    {
        public ILogger Logger { get; private set; }

        public TestAuthorizationFilter2(ILogger logger)
        {
            Logger = logger;
        }

        public Task OnAuthorizationAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}