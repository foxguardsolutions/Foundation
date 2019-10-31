using Autofac.Builder;

namespace FGS.Autofac.DynamicScoping.Abstractions
{
    /// <summary>
    /// Indicates the lifetime management semantics by which a component will be resolved.
    /// </summary>
    public enum Scope
    {
        /// <summary>
        /// Configure the component so that every dependent component or call to Resolve()
        /// gets a new, unique instance (default).
        /// </summary>
        /// <remarks>See <see cref="IRegistrationBuilder{TLimit,TActivatorData,TRegistrationStyle}.InstancePerDependency()"/>.</remarks>
        PerDependency = 1,

        /// <summary>
        /// Configure the component so that every dependent component or call to Resolve()
        /// within a single ILifetimeScope gets the same, shared instance. Dependent components in
        /// different lifetime scopes will get different instances.
        /// </summary>
        /// <remarks>See <see cref="IRegistrationBuilder{TLimit,TActivatorData,TRegistrationStyle}.InstancePerLifetimeScope()"/>.</remarks>
        PerLifetimeScope = 2,

        /// <summary>
        /// Configure the component so that every dependent component or call to Resolve()
        /// gets the same, shared instance.
        /// </summary>
        /// <remarks>See <see cref="IRegistrationBuilder{TLimit,TActivatorData,TRegistrationStyle}.SingleInstance()"/>.</remarks>
        Singleton = 3,
    }
}
