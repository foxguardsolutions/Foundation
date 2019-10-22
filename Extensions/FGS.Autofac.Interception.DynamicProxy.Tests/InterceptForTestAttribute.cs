using System;

namespace FGS.Autofac.Interception.DynamicProxy.Tests
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class InterceptForTestAttribute : Attribute
    {
    }
}
