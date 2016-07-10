using System;

using Autofac;
using Autofac.Core.Lifetime;
using Autofac.Integration.Mvc;

namespace FGS.Pump.Extensions.DI.Mvc.Tests
{
    /// <remarks>Taken and modified from: https://github.com/autofac/Autofac.Mvc/blob/e26ce3fe9ccc639f1349bcd8aee8e6e4ee066346/test/Autofac.Integration.Mvc.Test/StubLifetimeScopeProvider.cs </remarks>
    public class StubLifetimeScopeProvider : ILifetimeScopeProvider
    {
        private ILifetimeScope _lifetimeScope;

        public StubLifetimeScopeProvider(ILifetimeScope container)
        {
            ApplicationContainer = container;
        }

        public ILifetimeScope ApplicationContainer { get; }

        public ILifetimeScope GetLifetimeScope(Action<ContainerBuilder> configurationAction)
        {
            return _lifetimeScope ?? (_lifetimeScope = BuildLifetimeScope(configurationAction));
        }

        public void EndLifetimeScope() => _lifetimeScope?.Dispose();

        private ILifetimeScope BuildLifetimeScope(Action<ContainerBuilder> configurationAction)
        {
            return (configurationAction == null)
                       ? ApplicationContainer.BeginLifetimeScope(MatchingScopeLifetimeTags.RequestLifetimeScopeTag)
                       : ApplicationContainer.BeginLifetimeScope(MatchingScopeLifetimeTags.RequestLifetimeScopeTag, configurationAction);
        }
    }
}
