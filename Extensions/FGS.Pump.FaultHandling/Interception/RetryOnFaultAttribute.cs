using System;

namespace FGS.Pump.FaultHandling.Interception
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RetryOnFaultAttribute : Attribute
    {
    }
}