using System.Threading.Tasks;

namespace FGS.Pump.Extensions.DI.Interception
{
    /// <summary>
    /// Represents an asynchronous method invocation that has been intercepted.
    /// </summary>
    public interface IAsyncInvocation : IInvocationCommon
    {
        /// <summary>
        /// Execute the intercepted method invocation asynchronously.
        /// </summary>
        /// <returns>
        /// <para>A <see cref="System.Threading.Tasks.Task"/> to await the completion of the method invocation.</para>
        /// <para>The returned task will not contain the return value of the invocation. For that, use the ReturnValue property.</para>
        /// </returns>
        Task ProceedAsync();
    }
}
