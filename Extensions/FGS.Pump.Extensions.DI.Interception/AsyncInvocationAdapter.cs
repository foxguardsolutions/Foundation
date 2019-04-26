using System.Threading.Tasks;

using Castle.DynamicProxy;

using ICastleInterceptorInvocation = Castle.DynamicProxy.IInvocation;

namespace FGS.Pump.Extensions.DI.Interception
{
    internal class AsyncInvocationAdapter : InvocationAdapterBase, IAsyncInvocation
    {
        private readonly IInvocationProceedInfo _proceedInfo;

        public AsyncInvocationAdapter(ICastleInterceptorInvocation adapted)
            : base(adapted)
        {
            // Castle.DynamicProxy has a temporal coupling in its API. We have to call `CaptureProceedInfo()`
            // on the invocation before we enter a continuation in the interceptor. Calling it here assumes
            // that we will be constructing this adapter before we do any asynchronous work in the interceptor.
            _proceedInfo = adapted.CaptureProceedInfo();
        }

        public async Task ProceedAsync()
        {
            _proceedInfo.Invoke();
            await ((Task)Adapted.ReturnValue).ConfigureAwait(continueOnCapturedContext: false);
        }
    }
}
