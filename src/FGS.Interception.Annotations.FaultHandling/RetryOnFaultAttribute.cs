using System;

namespace FGS.Interception.Annotations.FaultHandling
{
    /// <summary>
    /// Indicates that the target type or member should be intercepted with retry-on-fault semantics.
    /// </summary>
    /// <remarks>Does not actually perform any interception - external wiring is required for that.</remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RetryOnFaultAttribute : Attribute
    {
    }
}
