using System;

using Autofac;
using Autofac.Core;

using FGS.Autofac.CompositionRoot.Abstractions;
using FGS.Autofac.DynamicScoping.Abstractions;
using FGS.Autofac.DynamicScoping.Abstractions.Specialized;
using FGS.Extensions.DependencyInjection.Autofac;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FGS.Extensions.Hosting.DependencyInjection.Autofac
{
    /// <summary>
    /// Extension methods for the <see cref="IHostBuilder"/> interface.
    /// </summary>
    /// <remarks>Taken and modified from: https://github.com/autofac/Autofac.AspNetCore/blob/e8254995519b96c568de194371edad4d2c20db0e/src/Autofac.Integration.AspNetCore/AutofacWebHostBuilderExtensions.cs. </remarks>
    public static class AutofacHostBuilderExtensions
    {
        /// <summary>
        /// Adds a custom implementation of Autofac's <see cref="IServiceProviderFactory{TContainerBuilder}"/> to the <see cref="IServiceCollection"/>
        /// being built by <paramref name="builder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IHostBuilder"/> instance being configured.</param>
        /// <param name="forEachModule">An optional action used to configure each <see cref="IModule"/> being added to the container.</param>
        /// <param name="configurationAction">An optional action used to configure the container.</param>
        /// <returns>The existing <see cref="IHostBuilder"/> instance.</returns>
        /// <typeparam name="TModulesProvider">The type of provider that can enumerate all of the Autofac modules to be registered.</typeparam>
        /// <example>
        /// <code>
        /// public static IWebHostBuilder CreateWebHostBuilder(string[] args) =&gt;
        ///     WebHost.CreateDefaultBuilder(args)
        ///         .UseAutofacWithModulesProvider&lt;MyAutofacModulesProvider&gt;()
        ///         .UseStartup&lt;Startup&gt;();
        /// </code>
        /// </example>
        public static IHostBuilder UseAutofacWithModulesProvider<TModulesProvider>(this IHostBuilder builder, Action<IModule> forEachModule = null, Action<ContainerBuilder> configurationAction = null)
            where TModulesProvider : IModulesProvider, new()
        {
            void ForEachModule(IModule module)
            {
                (module as IOverridableHttpScopeAutofacModule)?.SetHttpScope(Scope.PerLifetimeScope);
                forEachModule?.Invoke(module);
            }

            return builder.UseServiceProviderFactory(new ModulesProviderBasedAutofacServiceProviderFactory<TModulesProvider>(ForEachModule, configurationAction));
        }
    }
}
