using Ninject.Modules;

namespace FGS.Pump.Extensions.DI
{
    public interface IOverridableHttpScopeNinjectModule : INinjectModule
    {
        void SetHttpScope(Scope scope);
    }
}
