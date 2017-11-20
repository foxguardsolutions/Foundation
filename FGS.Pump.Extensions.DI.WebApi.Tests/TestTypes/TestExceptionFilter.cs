using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;

namespace FGS.Pump.Extensions.DI.WebApi.Tests
{
    /// <remarks>Taken and modified from: https://github.com/autofac/Autofac.WebApi/blob/f764f7e10694a57cf19c968c1ca5b6b998ba82c2/test/Autofac.Integration.WebApi.Test/TestTypes.cs </remarks>
    public class TestExceptionFilter : ICustomAutofacExceptionFilter
    {
        public ILogger Logger { get; private set; }

        public bool AllowMultiple { get; }

        public TestExceptionFilter(ILogger logger)
        {
            Logger = logger;
        }

        public Task OnExceptionAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}