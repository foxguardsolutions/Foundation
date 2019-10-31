using System;

namespace FGS.Interception.Abstractions
{
    /// <summary>
    /// Represents a method invocation that has been intercepted.
    /// </summary>
    public interface IInvocation : IInvocationCommon
    {
        /// <summary>
        /// Proceeds the call to the next interceptor in line, and ultimately to the target method.
        /// </summary>
        /// <remarks>
        /// <para>To get or set the invocation return value, use the <see cref="IInvocationCommon.ReturnValue"/> property.</para>
        /// <para>
        /// Since interface proxies without a target don't have the target implementation to proceed to, it is important, that the last interceptor does not call this method, otherwise a <see cref="NotImplementedException"/> will be thrown.
        /// </para>
        /// </remarks>
        void Proceed();
    }
}
