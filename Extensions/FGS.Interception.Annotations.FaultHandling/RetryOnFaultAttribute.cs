using System;

namespace FGS.Interception.Annotations.FaultHandling
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RetryOnFaultAttribute : Attribute
    {
    }
}