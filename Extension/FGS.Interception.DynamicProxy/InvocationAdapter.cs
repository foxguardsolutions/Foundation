using FGS.Interception.Abstractions;

using ICastleInterceptorInvocation = Castle.DynamicProxy.IInvocation;

namespace FGS.Interception.DynamicProxy
{
    public class InvocationAdapter : InvocationAdapterBase, IInvocation
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
