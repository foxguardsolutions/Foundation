using System.Collections.Generic;

using Autofac.Core;

namespace FGS.Autofac.CompositionRoot.Abstractions
{
    /// <summary>
    /// Represents the ability to enumerate Autofac modules that should be registered with an Autofac container builder.
    /// </summary>
    public interface IModulesProvider
    {
        /// <summary>
        /// Gets the Autofac modules that should be registered with an Autofac container builder.
        /// </summary>
        /// <returns>The Autofac modules that should be registered with an Autofac container builder.</returns>
        IEnumerable<IModule> GetModules();
    }
}
