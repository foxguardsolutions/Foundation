using System;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;

namespace FGS.Autofac.AspNetCore.Mvc.Routing
{
    /// <summary>
    /// An implementation of <see cref="IUrlHelper"/> that attempts to retains most of <see cref="IUrlHelper"/>'s functionality when used in a singleton context, when an <see cref="HttpContext"/> is likely available but <see cref="ActionContext"/> is not.
    /// </summary>
    /// <remarks>
    /// This implementation is bound by these constraints:
    ///  - <see cref="IHttpContextAccessor"/> can be used in singleton scope because ASP.NET Core registers it as such.
    ///  - <see cref="IActionContextAccesspr"/> cannot be used in singleton scope, because ASP.NET Core registers only register it in the scope of a request.
    /// </remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1055:Uri return values should not be strings", Justification = "A specific API is being implemented wherein we do not have control over the method names or return types")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1054:Uri parameters should not be strings", Justification = "A specific API is being implemented wherein we do not have control over the method parameters' names or types")]
    public class SingletonCompatibleUrlHelper : IUrlHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly LinkGenerator _linkGenerator;

        protected HttpContext HttpContext => _httpContextAccessor.HttpContext;

        public SingletonCompatibleUrlHelper(IHttpContextAccessor httpContextAccessor, LinkGenerator linkGenerator)
        {
            _httpContextAccessor = httpContextAccessor;
            _linkGenerator = linkGenerator;
        }

        /// <inheritdoc />
        public ActionContext ActionContext => throw new NotSupportedException();

        /// <inheritdoc />
        /// <remarks>Taken from: https://github.com/aspnet/AspNetCore/blob/7a26d27e8b7f67a1ac80532e5872bfde6c28f952/src/Mvc/Mvc.Core/src/Routing/UrlHelperBase.cs. </remarks>
        public virtual bool IsLocalUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return false;
            }

            // Allows "/" or "/foo" but not "//" or "/\".
            if (url[0] == '/')
            {
                // url is exactly "/"
                if (url.Length == 1)
                {
                    return true;
                }

                // url doesn't start with "//" or "/\"
                if (url[1] != '/' && url[1] != '\\')
                {
                    return true;
                }

                return false;
            }

            // Allows "~/" or "~/foo" but not "~//" or "~/\".
            if (url[0] == '~' && url.Length > 1 && url[1] == '/')
            {
                // url is exactly "~/"
                if (url.Length == 2)
                {
                    return true;
                }

                // url doesn't start with "~//" or "~/\"
                if (url[2] != '/' && url[2] != '\\')
                {
                    return true;
                }

                return false;
            }

            return false;
        }

        /// <inheritdoc />
        /// <remarks>Taken and modified from: https://github.com/aspnet/AspNetCore/blob/7a26d27e8b7f67a1ac80532e5872bfde6c28f952/src/Mvc/Mvc.Core/src/Routing/UrlHelperBase.cs. </remarks>
        public virtual string Content(string contentPath)
        {
            if (string.IsNullOrEmpty(contentPath))
            {
                return null;
            }
            else if (contentPath[0] == '~')
            {
                var segment = new PathString(contentPath.Substring(1));
                var applicationPath = HttpContext.Request.PathBase;

                return applicationPath.Add(segment).Value;
            }

            return contentPath;
        }

        /// <inheritdoc />
        public virtual string Link(string routeName, object values)
        {
            return RouteUrl(new UrlRouteContext()
            {
                RouteName = routeName,
                Values = values,
                Protocol = HttpContext.Request.Scheme,
                Host = HttpContext.Request.Host.ToUriComponent(),
            });
        }

        /// <inheritdoc />
        public virtual string Action(UrlActionContext actionContext)
        {
            if (string.IsNullOrWhiteSpace(actionContext.Host) && string.IsNullOrWhiteSpace(actionContext.Protocol))
            {
                return _linkGenerator.GetPathByAction(_httpContextAccessor.HttpContext, actionContext.Action, actionContext.Controller, actionContext.Values);
            }
            else
            {
                var hostString = actionContext.Host != null ? new HostString(actionContext.Host) : default(HostString?);
                return _linkGenerator.GetUriByAction(_httpContextAccessor.HttpContext, actionContext.Action, actionContext.Controller, actionContext.Values, actionContext.Protocol, hostString);
            }
        }

        /// <inheritdoc />
        public virtual string RouteUrl(UrlRouteContext routeContext)
        {
            if (string.IsNullOrWhiteSpace(routeContext.Host) && string.IsNullOrWhiteSpace(routeContext.Protocol))
            {
                return _linkGenerator.GetPathByRouteValues(_httpContextAccessor.HttpContext, routeContext.RouteName, routeContext.Values);
            }
            else
            {
                var hostString = routeContext.Host != null ? new HostString(routeContext.Host) : default(HostString?);
                return _linkGenerator.GetUriByRouteValues(_httpContextAccessor.HttpContext, routeContext.RouteName, routeContext.Values, routeContext.Protocol, hostString);
            }
        }
    }
}
