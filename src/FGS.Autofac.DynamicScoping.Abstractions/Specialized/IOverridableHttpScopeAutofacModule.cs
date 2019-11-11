using Autofac.Core;

namespace FGS.Autofac.DynamicScoping.Abstractions.Specialized
{
    /// <summary>
    /// Represents an Autofac module which must behave differently based on the lifetime management semantics of an HTTP request for the current container.
    /// </summary>
    public interface IOverridableHttpScopeAutofacModule : IModule
    {
        /// <summary>
        /// Sets the lifetime management semantics of HTTP dependencies being registered.
        /// </summary>
        /// <param name="scope">Indicates the lifetime management semantics by which the HTTP-dependent components will be resolved.</param>
        void SetHttpScope(Scope scope);
    }
}
