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
        private AutofacScope _httpScope = AutofacScope.PerRequest;

        public void SetHttpScope(AutofacScope scope)
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
            DI.ContainerBuilderExtensions.In(builder.Register(c => new HttpContextWrapper(HttpContext.Current)).As<HttpContextBase>(), _httpScope);

            // HttpContext properties
            DI.ContainerBuilderExtensions.In(builder.Register(c => c.Resolve<HttpContextBase>().Request).As<HttpRequestBase>(), _httpScope);

            DI.ContainerBuilderExtensions.In(builder.Register(c => c.Resolve<HttpContextBase>().Response).As<HttpResponseBase>(), _httpScope);

            DI.ContainerBuilderExtensions.In(builder.Register(c => c.Resolve<HttpContextBase>().Server).As<HttpServerUtilityBase>(), _httpScope);

            DI.ContainerBuilderExtensions.In(builder.Register(c => c.Resolve<HttpContextBase>().Session).As<HttpSessionStateBase>(), _httpScope);

            DI.ContainerBuilderExtensions.In(builder.Register(c => c.Resolve<HttpContextBase>().Application).As<HttpApplicationStateBase>(), _httpScope);

            // HttpRequest properties
            DI.ContainerBuilderExtensions.In(builder.Register(c => c.Resolve<HttpRequestBase>().Browser).As<HttpBrowserCapabilitiesBase>(), _httpScope);

            DI.ContainerBuilderExtensions.In(builder.Register(c => c.Resolve<HttpRequestBase>().Files).As<HttpFileCollectionBase>(), _httpScope);

            DI.ContainerBuilderExtensions.In(builder.Register(c => c.Resolve<HttpRequestBase>().RequestContext).As<RequestContext>(), _httpScope);

            // HttpResponse properties
            DI.ContainerBuilderExtensions.In(builder.Register(c => c.Resolve<HttpResponseBase>().Cache).As<HttpCachePolicyBase>(), _httpScope);

            // HostingEnvironment properties
            DI.ContainerBuilderExtensions.In(builder.Register(c => HostingEnvironment.VirtualPathProvider).As<VirtualPathProvider>(), _httpScope);

            // MVC types
            DI.ContainerBuilderExtensions.In(builder.Register(c => new UrlHelper(c.Resolve<RequestContext>())).As<UrlHelper>(), _httpScope);

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
