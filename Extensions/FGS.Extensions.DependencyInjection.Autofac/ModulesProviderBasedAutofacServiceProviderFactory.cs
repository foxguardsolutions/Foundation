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
    /// A factory for creating a <see cref="ContainerBuilder"/> and an <see cref="IServiceProvider" />.
    /// </summary>
    /// <remarks>Taken and modified from: https://github.com/autofac/Autofac.Extensions.DependencyInjection/blob/36d487749ef7184357bbc4d162bf425b8474eb36/src/Autofac.Extensions.DependencyInjection/AutofacServiceProviderFactory.cs </remarks>
    public class ModulesProviderBasedAutofacServiceProviderFactory<TModulesProvider> : IServiceProviderFactory<ContainerBuilder>
        where TModulesProvider : IModulesProvider, new()
    {
        private readonly Action<IModule> _forEachModule;
        private readonly Action<ContainerBuilder> _configurationAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutofacServiceProviderFactory"/> class.
        /// </summary>
        /// <param name="forEachModule">Action for each <see cref="IModule"/> that can provide additional configuration before it is registered.</param>
        /// <param name="configurationAction">Action on a <see cref="ContainerBuilder"/> that adds component registrations to the container.</param>
        public ModulesProviderBasedAutofacServiceProviderFactory(Action<IModule> forEachModule, Action<ContainerBuilder> configurationAction = null)
        {
            _forEachModule = forEachModule;
            _configurationAction = configurationAction ?? (builder => { });
        }

        /// <summary>
        /// Creates a container builder from an <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <returns>A container builder that can be used to create an <see cref="IServiceProvider" />.</returns>
        public ContainerBuilder CreateBuilder(IServiceCollection services)
        {
            var builder = new ContainerBuilder();

            builder.Populate(services);

            builder.Populate<TModulesProvider>(_forEachModule);

            _configurationAction(builder);

            return builder;
        }

        /// <summary>
        /// Creates an <see cref="IServiceProvider" /> from the container builder.
        /// </summary>
        /// <param name="containerBuilder">The container builder.</param>
        /// <returns>An <see cref="IServiceProvider" />.</returns>
        public IServiceProvider CreateServiceProvider(ContainerBuilder containerBuilder)
        {
            if (containerBuilder == null) throw new ArgumentNullException(nameof(containerBuilder));

            var container = containerBuilder.Build();

            return new AutofacServiceProvider(container);
        }
    }
}
