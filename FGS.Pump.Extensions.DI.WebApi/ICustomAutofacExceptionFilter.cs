using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;

namespace FGS.Pump.Extensions.DI.WebApi
{
    /// <summary>
    /// An exception filter that will be created for each controller request.
    /// </summary>
    /// <remarks>Taken and modified from: https://github.com/autofac/Autofac.WebApi/blob/e3e83d6506c137e6cacbc5fdb57634d7a388a5e9/src/Autofac.Integration.WebApi/IAutofacExceptionFilter.cs </remarks>
    public interface ICustomAutofacExceptionFilter
    {
        /// <summary>
        /// Called when an exception is thrown.
        /// </summary>
        /// <param name="actionExecutedContext">The context for the action.</param>
        /// <param name="cancellationToken">A cancellation token for signaling task ending.</param>
        Task OnExceptionAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken);
    }
}
