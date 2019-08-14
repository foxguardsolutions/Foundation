using System;

namespace FGS.Pump.Extensions.DI.Interception.Tests
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class InterceptForTestAttribute : Attribute
    {
    }
}
