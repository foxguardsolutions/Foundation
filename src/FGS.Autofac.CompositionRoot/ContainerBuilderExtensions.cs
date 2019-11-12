using System;

using Autofac;
using Autofac.Core;

using FGS.Autofac.CompositionRoot.Abstractions;

namespace FGS.Autofac.CompositionRoot
{
    /// <summary>
    /// Extends <see cref="ContainerBuilder"/> with functionality to populate it en-masse from implementations of <see cref="IModulesProvider" />.
    /// </summary>
    public static class ContainerBuilderExtensions
    {
        /// <summary>
        /// Populates the given <paramref name="containerBuilder"/> with all the modules provided by <typeparamref name="TAutofacModulesProvider"/>,
        /// optionally applying logic from <paramref name="forEachModule"/> on each module before adding it to the <paramref name="containerBuilder"/>.
        /// </summary>
        /// <typeparam name="TAutofacModulesProvider">The type of provider that can enumerate all of the Autofac modules to be registered.</typeparam>
        /// <param name="containerBuilder">The container builder to be populated with modules.</param>
        /// <param name="forEachModule">An optional action used to configure each <see cref="IModule"/> being added to the container builder.</param>
        public static void Populate<TAutofacModulesProvider>(this ContainerBuilder containerBuilder, Action<IModule> forEachModule = null)
            where TAutofacModulesProvider : IModulesProvider, new()
        {
            var modulesProvider = new TAutofacModulesProvider();
            foreach (var module in modulesProvider.GetModules())
            {
                forEachModule?.Invoke(module);

                containerBuilder.RegisterModule(module);
            }
        }
    }
}
