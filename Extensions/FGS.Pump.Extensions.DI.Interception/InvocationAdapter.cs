using ICastleInterceptorInvocation = Castle.DynamicProxy.IInvocation;

namespace FGS.Pump.Extensions.DI.Interception
{
    internal class InvocationAdapter : InvocationAdapterBase, IInvocation
    {
        public InvocationAdapter(ICastleInterceptorInvocation adapted)
            : base(adapted)
        {
        }

        public void Proceed()
        {
            Adapted.Proceed();
        }
    }
}
