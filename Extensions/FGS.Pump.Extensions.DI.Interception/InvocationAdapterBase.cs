using System;
using System.Reflection;

using ICastleInterceptorInvocation = Castle.DynamicProxy.IInvocation;
namespace FGS.Pump.Extensions.DI.Interception
{
    internal abstract class InvocationAdapterBase : IInvocationCommon
    {
        protected ICastleInterceptorInvocation Adapted { get; }

        protected InvocationAdapterBase(ICastleInterceptorInvocation adapted)
        {
            Adapted = adapted;
        }

        public object GetArgumentValue(int index) => Adapted.GetArgumentValue(index);

        public MethodInfo GetConcreteMethod() => Adapted.GetConcreteMethod();

        public MethodInfo GetConcreteMethodInvocationTarget() => Adapted.GetConcreteMethodInvocationTarget();

        public void SetArgumentValue(int index, object value) => Adapted.SetArgumentValue(index, value);

        public object[] Arguments => Adapted.Arguments;

        public Type[] GenericArguments => Adapted.GenericArguments;

        public object InvocationTarget => Adapted.InvocationTarget;

        public MethodInfo Method => Adapted.Method;

        public MethodInfo MethodInvocationTarget => Adapted.MethodInvocationTarget;

        public object Proxy => Adapted.Proxy;

        public object ReturnValue
        {
            get => Adapted.ReturnValue;
            set => Adapted.ReturnValue = value;
        }

        public Type TargetType => Adapted.TargetType;
    }
}
