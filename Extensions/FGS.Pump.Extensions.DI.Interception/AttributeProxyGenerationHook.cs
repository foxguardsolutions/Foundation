using System;
using System.Reflection;

using Castle.DynamicProxy;

namespace FGS.Pump.Extensions.DI.Interception
{
    public class AttributeProxyGenerationHook<TAttribute> : IProxyGenerationHook
        where TAttribute : Attribute
    {
        public void MethodsInspected()
        {
        }

        public void NonProxyableMemberNotification(Type type, MemberInfo memberInfo)
        {
        }

        public bool ShouldInterceptMethod(Type type, MethodInfo methodInfo)
        {
            return type.HasAttribute<TAttribute>() || methodInfo.HasAttribute<TAttribute>();
        }
    }
}
