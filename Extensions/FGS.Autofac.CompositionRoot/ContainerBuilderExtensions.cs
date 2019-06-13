using System;

using Autofac;
using Autofac.Core;

using FGS.Autofac.CompositionRoot.Abstractions;

namespace FGS.Autofac.CompositionRoot
{
    public static class ContainerBuilderExtensions
    {
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
