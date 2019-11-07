using System;

using Autofac;
using Autofac.Core;

using FGS.Autofac.CompositionRoot.Abstractions;
using FGS.Autofac.DynamicScoping.Abstractions;
using FGS.Autofac.DynamicScoping.Abstractions.Specialized;
using FGS.Extensions.DependencyInjection.Autofac;

using Microsoft.Extensions.DependencyInjection;

#if NET472 || NETSTANDARD2_0
using IWebHostBuilder = Microsoft.AspNetCore.Hosting.IWebHostBuilder;
#elif NETSTANDARD2_1 || NETCOREAPP3_0
using Microsoft.Extensions.Hosting;
using IWebHostBuilder = Microsoft.Extensions.Hosting.IHostBuilder;
#endif

#if NET472 || NETSTANDARD2_0
namespace FGS.AspNetCore.Hosting.Extensions.DependencyInjection.Autofac
#elif NETSTANDARD2_1 || NETCOREAPP3_0
namespace FGS.Extensions.Hosting.DependencyInjection.Autofac
#endif
{
    /// <summary>
    /// Extension methods for the <see cref="IWebHostBuilder"/> interface.
    /// </summary>
    /// <remarks>Taken and modified from: https://github.com/autofac/Autofac.AspNetCore/blob/e8254995519b96c568de194371edad4d2c20db0e/src/Autofac.Integration.AspNetCore/AutofacWebHostBuilderExtensions.cs. </remarks>
#if NET472 || NETSTANDARD2_0
    public static class AutofacWebHostBuilderExtensions
#elif NETSTANDARD2_1 || NETCOREAPP3_0
    public static class AutofacHostBuilderExtensions
#endif
    {
        /// <summary>
        /// Adds a custom implementation of Autofac's <see cref="IServiceProviderFactory{TContainerBuilder}"/> to the <see cref="IServiceCollection"/>
        /// being built by <paramref name="builder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IWebHostBuilder"/> instance being configured.</param>
        /// <param name="forEachModule">An optional action used to configure each <see cref="IModule"/> being added to the container.</param>
        /// <param name="configurationAction">An optional action used to configure the container.</param>
        /// <returns>The existing <see cref="IWebHostBuilder"/> instance.</returns>
        /// <typeparam name="TModulesProvider">The type of provider that can enumerate all of the Autofac modules to be registered.</typeparam>
        /// <example>
        /// <code>
        /// public static IWebHostBuilder CreateWebHostBuilder(string[] args) =&gt;
        ///     WebHost.CreateDefaultBuilder(args)
        ///         .UseAutofacWithModulesProvider&lt;MyAutofacModulesProvider&gt;()
        ///         .UseStartup&lt;Startup&gt;();
        /// </code>
        /// </example>
        public static IWebHostBuilder UseAutofacWithModulesProvider<TModulesProvider>(this IWebHostBuilder builder, Action<IModule> forEachModule = null, Action<ContainerBuilder> configurationAction = null)
            where TModulesProvider : IModulesProvider, new()
        {
            void ForEachModule(IModule module)
            {
                (module as IOverridableHttpScopeAutofacModule)?.SetHttpScope(Scope.PerLifetimeScope);
                forEachModule?.Invoke(module);
            }
#if NET472 || NETSTANDARD2_0
            return builder.ConfigureServices(services => services.AddAutofacWithModulesProvider<TModulesProvider>(ForEachModule, configurationAction));

#elif NETSTANDARD2_1 || NETCOREAPP3_0
            return builder.UseServiceProviderFactory(new ModulesProviderBasedAutofacServiceProviderFactory<TModulesProvider>(ForEachModule, configurationAction));
#endif
        }
    }
}
