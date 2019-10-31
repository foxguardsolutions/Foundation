using System.Reflection;

namespace FGS.Interception.Abstractions
{
    /// <summary>
    /// Represents the commonalities between synchronous and asynchronous method invocations that are being intercepted.
    /// </summary>
    /// <remarks>Despite this API being heavily based on that of `Castle.DynamicProxy`, <c>CaptureProceedInfo</c> is purposefully excluded because
    /// it is only exposed by `Castle.DynamicProxy` in order to let consumers opt-in to thread safety. Thread-safety should be considered the default
    /// when implementing _this_ API.</remarks>
    public interface IInvocationCommon
    {
        /// <summary>
        /// Gets the value of the argument, of the intercepted method invocation, at the given index.
        /// </summary>
        /// <param name="index">The index of the argument, of the intercepted method invocation, to get.</param>
        /// <returns>The value of the specified argument.</returns>
        object GetArgumentValue(int index);

        /// <summary>
        /// Returns the concrete instantiation of <see cref="Method"/> on the proxy, with any generic parameters bound to real types.
        /// </summary>
        /// <returns>The concrete instantiation of <see cref="Method"/> on the proxy if a generic method is being intercepted, or otherwise the same value as <see cref="Method"/>.</returns>
        System.Reflection.MethodInfo GetConcreteMethod();

        /// <summary>
        /// Returns the concrete instantiation of <see cref="MethodInvocationTarget"/>, with any generic parameters bound to real types. For interface proxies, this will point to the <see cref="MethodInfo"/> on the target class.
        /// </summary>
        /// <returns>The concrete instantiation of <see cref="MethodInvocationTarget"/> if a generic method is being intercepted, or otherwise the same value as <see cref="MethodInvocationTarget"/>.</returns>
        System.Reflection.MethodInfo GetConcreteMethodInvocationTarget();

        /// <summary>
        /// Overrides the value of an argument at the given index with the new value provided.
        /// </summary>
        /// <param name="index">The index of the argument to override.</param>
        /// <param name="value">The new value for the argument.</param>
        /// <remarks>This method accepts an <see cref="object"/>, however the value provided must be compatible with the type of the argument defined on the method, otherwise an exception will be thrown.</remarks>
        void SetArgumentValue(int index, object value);

        /// <summary>
        /// Gets the arguments that the intercepted method has been invoked with.
        /// </summary>
        object[] Arguments { get; }

        /// <summary>
        /// Gets the generic arguments of the intercepted method.
        /// </summary>
        System.Type[] GenericArguments { get; }

        /// <summary>
        /// Gets the object on which the invocation is performed. This is different from proxy object because most of the time this will be the proxy target object.
        /// </summary>
        object InvocationTarget { get; }

        /// <summary>
        /// Gets the <see cref="MethodInfo"/> representing the method being invoked on the proxy.
        /// </summary>
        System.Reflection.MethodInfo Method { get; }

        /// <summary>
        /// Gets the <see cref="MethodInfo"/> - for interface proxies - on the target class.
        /// </summary>
        System.Reflection.MethodInfo MethodInvocationTarget { get; }

        /// <summary>
        /// Gets the proxy object on which the intercepted method is invoked.
        /// </summary>
        object Proxy { get; }

        /// <summary>
        /// Gets or sets the return value of the intercepted method.
        /// </summary>
        object ReturnValue { get; set; }

        /// <summary>
        /// Gets the type of the target object for the intercepted method.
        /// </summary>
        System.Type TargetType { get; }
    }
}
