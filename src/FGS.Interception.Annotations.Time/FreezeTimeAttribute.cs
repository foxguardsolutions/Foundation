using System;

namespace FGS.Interception.Annotations.Time
{
    /// <summary>
    /// Indicates that the target type or member should be intercepted with time-freezing semantics.
    /// </summary>
    /// <remarks>Does not actually perform any interception - external wiring is required for that.</remarks>
    [AttributeUsage(AttributeTargets.Method)]
    public class FreezeTimeAttribute : Attribute
    {
    }
}
