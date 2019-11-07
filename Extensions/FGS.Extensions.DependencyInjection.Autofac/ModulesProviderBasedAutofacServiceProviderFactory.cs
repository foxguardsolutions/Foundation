using System;

using Autofac;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;

using FGS.Autofac.CompositionRoot;
using FGS.Autofac.CompositionRoot.Abstractions;

using Microsoft.Extensions.DependencyInjection;

namespace FGS.Extensions.DependencyInjection.Autofac
{
    /// <summary>
    /// A factory for creating an Autofac-based <see cref="IServiceProvider" />.
    /// </summary>
    /// <typeparam name="TModulesProvider">The type of provider that can enumerate all of the Autofac modules to be registered.</typeparam>
    /// <remarks>Taken and modified from: https://github.com/autofac/Autofac.Extensions.DependencyInjection/blob/36d487749ef7184357bbc4d162bf425b8474eb36/src/Autofac.Extensions.DependencyInjection/AutofacServiceProviderFactory.cs. </remarks>
    public class ModulesProviderBasedAutofacServiceProviderFactory<TModulesProvider> : IServiceProviderFactory<IServiceCollection>
        where TModulesProvider : IModulesProvider, new()
    {
        private readonly Action<IModule> _forEachModule;
        private readonly Action<ContainerBuilder> _configurationAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModulesProviderBasedAutofacServiceProviderFactory{TModulesProvider}"/> class.
        /// </summary>
        /// <param name="forEachModule">Action for each <see cref="IModule"/> that can provide additional configuration before it is registered.</param>
        /// <param name="configurationAction">Action on a <see cref="ContainerBuilder"/> that adds component registrations to the container.</param>
        public ModulesProviderBasedAutofacServiceProviderFactory(Action<IModule> forEachModule = null, Action<ContainerBuilder> configurationAction = null)
        {
            _forEachModule = forEachModule;
            _configurationAction = configurationAction ?? (builder => { });
        }

        /// <summary>
        /// Creates a container builder from an <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <returns>A service collection that can be used to create an <see cref="IServiceProvider" />.</returns>
        public IServiceCollection CreateBuilder(IServiceCollection services) => services;

        /// <summary>
        /// Creates an <see cref="IServiceProvider" /> from the given set of services.
        /// </summary>
        /// <param name="services">The the services to create an <see cref="IServiceProvider" /> from.</param>
        /// <returns>An <see cref="IServiceProvider" />.</returns>
        public IServiceProvider CreateServiceProvider(IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            var builder = new ContainerBuilder();

            builder.Populate(services);

            builder.Populate<TModulesProvider>(_forEachModule);

            _configurationAction(builder);

            var container = builder.Build();

            return new AutofacServiceProvider(container);
        }
    }
}
