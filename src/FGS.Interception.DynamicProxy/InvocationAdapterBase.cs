using System;
using System.Reflection;

using FGS.Interception.Abstractions;

using ICastleInterceptorInvocation = Castle.DynamicProxy.IInvocation;

namespace FGS.Interception.DynamicProxy
{
    /// <inheritdoc cref="IInvocationCommon" />
    public abstract class InvocationAdapterBase : IInvocationCommon
    {
        protected ICastleInterceptorInvocation Adapted { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvocationAdapterBase"/> class.
        /// </summary>
        /// <param name="adapted">The underlying implementation that is being adapted.</param>
        internal InvocationAdapterBase(ICastleInterceptorInvocation adapted)
        {
            Adapted = adapted;
        }

        /// <inheritdoc />
        public object GetArgumentValue(int index) => Adapted.GetArgumentValue(index);

        /// <inheritdoc />
        public MethodInfo GetConcreteMethod() => Adapted.GetConcreteMethod();

        /// <inheritdoc />
        public MethodInfo GetConcreteMethodInvocationTarget() => Adapted.GetConcreteMethodInvocationTarget();

        /// <inheritdoc />
        public void SetArgumentValue(int index, object value) => Adapted.SetArgumentValue(index, value);

        /// <inheritdoc />
        public object[] Arguments => Adapted.Arguments;

        /// <inheritdoc />
        public Type[] GenericArguments => Adapted.GenericArguments;

        /// <inheritdoc />
        public object InvocationTarget => Adapted.InvocationTarget;

        /// <inheritdoc />
        public MethodInfo Method => Adapted.Method;

        /// <inheritdoc />
        public MethodInfo MethodInvocationTarget => Adapted.MethodInvocationTarget;

        /// <inheritdoc />
        public object Proxy => Adapted.Proxy;

        /// <inheritdoc />
        public object ReturnValue
        {
            get => Adapted.ReturnValue;
            set => Adapted.ReturnValue = value;
        }

        /// <inheritdoc />
        public Type TargetType => Adapted.TargetType;
    }
}
