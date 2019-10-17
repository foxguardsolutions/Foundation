using System.Threading.Tasks;

namespace FGS.Interception.Abstractions
{
    /// <summary>
    /// Defines a discrete unit of behavior executed as part of the process of intercepting the execution of a member of a type.
    /// </summary>
    public interface IInterceptor
    {
        void Intercept(IInvocation invocation);

        Task InterceptAsync(IAsyncInvocation invocation);
    }
}
