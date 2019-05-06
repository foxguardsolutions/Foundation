using System.Threading.Tasks;

namespace FGS.Pump.Extensions.DI.Interception
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
