using Autofac;

using FGS.Autofac.CompositionRoot;
using FGS.Autofac.CompositionRoot.Abstractions;
using FGS.Autofac.DynamicScoping.Abstractions;
using FGS.Autofac.DynamicScoping.Abstractions.Specialized;

namespace FGS.Autofac.DynamicScoping
{
    public static class ContainerBuilderFactory
    {
        public static ContainerBuilder Create<TAutofacModulesProvider>(Scope httpScope)
            where TAutofacModulesProvider : IModulesProvider, new()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.Populate<TAutofacModulesProvider>(m => (m as IOverridableHttpScopeAutofacModule)?.SetHttpScope(httpScope));
            return containerBuilder;
        }
    }
}
