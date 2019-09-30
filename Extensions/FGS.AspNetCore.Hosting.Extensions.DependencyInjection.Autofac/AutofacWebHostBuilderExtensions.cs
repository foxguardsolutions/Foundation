using System;

using Autofac;
using Autofac.Core;

using FGS.Autofac.CompositionRoot.Abstractions;
using FGS.Autofac.DynamicScoping.Abstractions;
using FGS.Autofac.DynamicScoping.Abstractions.Specialized;
using FGS.Extensions.DependencyInjection.Autofac;

using Microsoft.Extensions.DependencyInjection;

#if NETSTANDARD2_0
using IWebHostBuilder = Microsoft.AspNetCore.Hosting.IWebHostBuilder;
#elif NETSTANDARD2_1
using Microsoft.Extensions.Hosting;
using IWebHostBuilder = Microsoft.Extensions.Hosting.IHostBuilder;
#endif

namespace FGS.AspNetCore.Hosting.Extensions.DependencyInjection.Autofac
{
    /// <summary>
    /// Extension methods for the <see cref="IWebHostBuilder"/> interface.
    /// </summary>
    /// <remarks>Taken and modified from: https://github.com/autofac/Autofac.AspNetCore/blob/e8254995519b96c568de194371edad4d2c20db0e/src/Autofac.Integration.AspNetCore/AutofacWebHostBuilderExtensions.cs. </remarks>
    public static class AutofacWebHostBuilderExtensions
    {
        /// <summary>
        /// Adds our custom implementation of Autofac's <see cref="IServiceProviderFactory{TContainerBuilder}"/> to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IWebHostBuilder"/> instance being configured.</param>
        /// <param name="forEachModule">An option action used to configure each <see cref="IModule"/> being added to the container.</param>
        /// <param name="configurationAction">An option action used to configure the container.</param>
        /// <returns>The existing <see cref="IWebHostBuilder"/> instance.</returns>
        public static IWebHostBuilder UseAutofacWithModulesProvider<TModulesProvider>(this IWebHostBuilder builder, Action<IModule> forEachModule = null, Action<ContainerBuilder> configurationAction = null)
            where TModulesProvider : IModulesProvider, new()
        {
            void ForEachModule(IModule module)
            {
                (module as IOverridableHttpScopeAutofacModule)?.SetHttpScope(Scope.PerLifetimeScope);
                forEachModule?.Invoke(module);
            }

            return builder.ConfigureServices(services => services.AddAutofacWithModulesProvider<TModulesProvider>(ForEachModule, configurationAction));
        }
    }
}
