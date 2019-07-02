using System;

using Autofac;
using Autofac.Core;

using FGS.Autofac.CompositionRoot.Abstractions;

using Microsoft.Extensions.DependencyInjection;

namespace FGS.Extensions.DependencyInjection.Autofac
{
    /// <summary>
    /// Extension methods on <see cref="IServiceCollection"/> to register the <see cref="IServiceProviderFactory{TContainerBuilder}"/>.
    /// </summary>
    /// <remarks>Taken and modified from: https://github.com/autofac/Autofac.Extensions.DependencyInjection/blob/36d487749ef7184357bbc4d162bf425b8474eb36/src/Autofac.Extensions.DependencyInjection/ServiceCollectionExtensions.cs </remarks>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the <see cref="ModulesProviderBasedAutofacServiceProviderFactory"/> to the service collection.
        /// </summary>
        /// <param name="services">The service collection to add the factory to.</param>
        /// <param name="forEachModule">Action for each <see cref="IModule"/> that can provide additional configuration before it is registered.</param>
        /// <param name="configurationAction">Action on a <see cref="ContainerBuilder"/> that adds component registrations to the container.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddAutofacWithModulesProvider<TModulesProvider>(this IServiceCollection services, Action<IModule> forEachModule = null, Action<ContainerBuilder> configurationAction = null)
            where TModulesProvider : IModulesProvider, new()
        {
            return services.AddSingleton<IServiceProviderFactory<ContainerBuilder>>(new ModulesProviderBasedAutofacServiceProviderFactory<TModulesProvider>(forEachModule, configurationAction));
        }

    }
}
