using System.Collections.Generic;

using Autofac.Core;

namespace FGS.Autofac.CompositionRoot.Abstractions
{
    public interface IModulesProvider
    {
        IEnumerable<IModule> GetModules();
    }
}
