using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;

namespace FGS.Pump.Extensions.DI.WebApi.Tests
{
    /// <remarks>Taken and modified from: https://github.com/autofac/Autofac.WebApi/blob/f764f7e10694a57cf19c968c1ca5b6b998ba82c2/test/Autofac.Integration.WebApi.Test/TestTypes.cs </remarks>
    public class TestAuthorizationFilter : ICustomAutofacAuthorizationFilter
    {
        public ILogger Logger { get; private set; }

        public TestAuthorizationFilter(ILogger logger)
        {
            Logger = logger;
        }

        public Task OnAuthorizationAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            return Task.WhenAll();
        }
    }
}