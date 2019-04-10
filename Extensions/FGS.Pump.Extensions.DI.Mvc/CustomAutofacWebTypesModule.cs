using System.Diagnostics.CodeAnalysis;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;

using Autofac;
using Autofac.Integration.Mvc;

namespace FGS.Pump.Extensions.DI.Mvc
{
    /// <remarks>Taken and modified from: https://github.com/autofac/Autofac.Mvc/blob/e26ce3fe9ccc639f1349bcd8aee8e6e4ee066346/src/Autofac.Integration.Mvc/AutofacWebTypesModule.cs </remarks>
    public class CustomAutofacWebTypesModule : Module, IOverridableHttpScopeAutofacModule
    {
        private Scope _httpScope = Scope.PerRequest;

        public void SetHttpScope(Scope scope)
        {
            _httpScope = scope;
        }

        /// <summary>
        /// Registers web abstractions with dependency injection.
        /// </summary>
        /// <param name="builder">
        /// The <see cref="Autofac.ContainerBuilder"/> in which registration
        /// should take place.
        /// </param>
        /// <remarks>
        /// <para>
        /// This method registers mappings between common current context-related
        /// web constructs and their associated abstract counterparts. See
        /// <see cref="Autofac.Integration.Mvc.AutofacWebTypesModule"/> for the complete
        /// list of mappings that get registered.
        /// </para>
        /// </remarks>
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "A lot of types get registered, but there isn't much complexity.")]
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "The complexity is in the registration lambdas. They're not actually hard to maintain.")]
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c => new HttpContextWrapper(HttpContext.Current)).As<HttpContextBase>().In(_httpScope);

            // HttpContext properties
            builder.Register(c => c.Resolve<HttpContextBase>().Request).As<HttpRequestBase>().In(_httpScope);

            builder.Register(c => c.Resolve<HttpContextBase>().Response).As<HttpResponseBase>().In(_httpScope);

            builder.Register(c => c.Resolve<HttpContextBase>().Server).As<HttpServerUtilityBase>().In(_httpScope);

            builder.Register(c => c.Resolve<HttpContextBase>().Session).As<HttpSessionStateBase>().In(_httpScope);

            builder.Register(c => c.Resolve<HttpContextBase>().Application).As<HttpApplicationStateBase>().In(_httpScope);

            // HttpRequest properties
            builder.Register(c => c.Resolve<HttpRequestBase>().Browser).As<HttpBrowserCapabilitiesBase>().In(_httpScope);

            builder.Register(c => c.Resolve<HttpRequestBase>().Files).As<HttpFileCollectionBase>().In(_httpScope);

            builder.Register(c => c.Resolve<HttpRequestBase>().RequestContext).As<RequestContext>().In(_httpScope);

            // HttpResponse properties
            builder.Register(c => c.Resolve<HttpResponseBase>().Cache).As<HttpCachePolicyBase>().In(_httpScope);

            // HostingEnvironment properties
            builder.Register(c => HostingEnvironment.VirtualPathProvider).As<VirtualPathProvider>().In(_httpScope);

            // MVC types
            builder.Register(c => new UrlHelper(c.Resolve<RequestContext>())).As<UrlHelper>().In(_httpScope);

            builder.Register(c => System.Web.Routing.RouteTable.Routes)
                .AsSelf()
                .SingleInstance();

            builder.Register(c => System.Web.Mvc.ModelBinderProviders.BinderProviders).AsSelf().SingleInstance();

            builder.RegisterSource(new ViewRegistrationSource());

            builder.Register(c => new CustomAutofacFilterProvider()).As<IFilterProvider>().SingleInstance();

            builder.Register(c => new CachedDataAnnotationsModelMetadataProvider()).As<ModelMetadataProvider>().SingleInstance();

            builder.Register(c => ModelValidatorProviders.Providers).AsSelf().SingleInstance();
        }
    }
}
