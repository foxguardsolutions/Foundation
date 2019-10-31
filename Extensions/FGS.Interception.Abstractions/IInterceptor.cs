using System.Threading.Tasks;

namespace FGS.Interception.Abstractions
{
    /// <summary>
    /// Defines a discrete unit of behavior executed as part of the process of intercepting the execution of a member of a type.
    /// </summary>
    public interface IInterceptor
    {
        /// <summary>
        /// The implementation that is invoked to intercept a member with a synchronous signature.
        /// </summary>
        /// <param name="invocation">Represents a method invocation that has been intercepted.</param>
        void Intercept(IInvocation invocation);

        /// <summary>
        /// The implementation that is invoked to intercept a member with an asynchronous signature.
        /// </summary>
        /// <param name="invocation">Represents an asynchronous method invocation that has been intercepted.</param>
        /// <returns>A <see cref="Task"/> that contains the continuation remaining from end of the intercepted invocation.</returns>
        Task InterceptAsync(IAsyncInvocation invocation);
    }
}
