using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;

namespace FGS.Pump.Extensions.DI.WebApi
{
    /// <summary>
    /// An authorization filter that will be created for each controller request.
    /// </summary>
    /// <remarks>Taken and modified from: https://github.com/autofac/Autofac.WebApi/blob/e3e83d6506c137e6cacbc5fdb57634d7a388a5e9/src/Autofac.Integration.WebApi/IAutofacAuthorizationFilter.cs </remarks>
    public interface ICustomAutofacAuthorizationFilter
    {
        /// <summary>
        /// Called when a process requests authorization.
        /// </summary>
        /// <param name="actionContext">The context for the action.</param>
        /// <param name="cancellationToken">A cancellation token for signaling task ending.</param>
        Task OnAuthorizationAsync(HttpActionContext actionContext, CancellationToken cancellationToken);
    }
}
