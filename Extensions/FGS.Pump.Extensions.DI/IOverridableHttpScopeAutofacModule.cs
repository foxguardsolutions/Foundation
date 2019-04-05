using Autofac.Core;

namespace FGS.Pump.Extensions.DI
{
    public interface IOverridableHttpScopeAutofacModule : IModule
    {
        void SetHttpScope(Scope scope);
    }
}
