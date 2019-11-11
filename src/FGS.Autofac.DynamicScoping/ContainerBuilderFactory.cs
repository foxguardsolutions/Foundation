using Autofac;

using FGS.Autofac.CompositionRoot;
using FGS.Autofac.CompositionRoot.Abstractions;
using FGS.Autofac.DynamicScoping.Abstractions;
using FGS.Autofac.DynamicScoping.Abstractions.Specialized;

namespace FGS.Autofac.DynamicScoping
{
    /// <summary>
    /// Provides the ability to create a <see cref="ContainerBuilder"/>.
    /// </summary>
    public static class ContainerBuilderFactory
    {
        /// <summary>
        /// Creates a <see cref="ContainerBuilder"/>, which has been populated via <typeparamref name="TAutofacModulesProvider"/>, and had
        /// all instances of <see cref="IOverridableHttpScopeAutofacModule"/> configured to have the scope of <paramref name="httpScope"/>.
        /// </summary>
        /// <param name="httpScope">The <see cref="Scope"/> that all web-dependent registrations should be registered in.</param>
        /// <typeparam name="TAutofacModulesProvider">The type of provider that can enumerate all of the Autofac modules to be registered.</typeparam>
        /// <returns>The created and populated <see cref="ContainerBuilder"/>.</returns>
        public static ContainerBuilder Create<TAutofacModulesProvider>(Scope httpScope)
            where TAutofacModulesProvider : IModulesProvider, new()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.Populate<TAutofacModulesProvider>(m => (m as IOverridableHttpScopeAutofacModule)?.SetHttpScope(httpScope));
            return containerBuilder;
        }
    }
}
