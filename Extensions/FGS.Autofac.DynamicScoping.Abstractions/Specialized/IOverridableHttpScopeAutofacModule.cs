using Autofac.Core;

namespace FGS.Autofac.DynamicScoping.Abstractions.Specialized
{
    public interface IOverridableHttpScopeAutofacModule : IModule
    {
        void SetHttpScope(Scope scope);
    }
}
