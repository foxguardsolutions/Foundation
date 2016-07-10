using Autofac.Core;

namespace FGS.Pump.Extensions.DI
{
#pragma warning disable CS0436 // Type conflicts with imported type
    public interface IOverridableHttpScopeAutofacModule : IModule
    {
        void SetHttpScope(AutofacScope scope);
    }
}
