using System.Threading.Tasks;

namespace FGS.Interception.Abstractions
{
    /// <summary>
    /// Represents an asynchronous method invocation that has been intercepted.
    /// </summary>
    public interface IAsyncInvocation : IInvocationCommon
    {
        /// <summary>
        /// Asynchronously proceeds the call to the next interceptor in line, and ultimately to the target method.
        /// </summary>
        /// <returns>
        /// <para>A <see cref="System.Threading.Tasks.Task"/> to await the completion of the method invocation.</para>
        /// <para>The returned task will not contain the return value of the invocation. For that, use the <see cref="IInvocationCommon.ReturnValue"/> property.</para>
        /// </returns>
        /// <remarks>
        /// <para>To get or set the invocation return value, use the <see cref="IInvocationCommon.ReturnValue"/> property.</para>
        /// <para>
        /// Since interface proxies without a target don't have the target implementation to proceed to, it is important, that the last interceptor does not call this method, otherwise a <see cref="NotImplementedException"/> will be thrown.
        /// </para>
        /// </remarks>
        Task ProceedAsync();
    }
}
