using FGS.Interception.Abstractions;

using ICastleInterceptorInvocation = Castle.DynamicProxy.IInvocation;

namespace FGS.Interception.DynamicProxy
{
    /// <inheritdoc cref="IInvocation" />
    public sealed class InvocationAdapter : InvocationAdapterBase, IInvocation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvocationAdapter"/> class.
        /// </summary>
        /// <param name="adapted">The underlying implementation that is being adapted.</param>
        public InvocationAdapter(ICastleInterceptorInvocation adapted)
            : base(adapted)
        {
        }

        /// <inheritdoc />
        public void Proceed()
        {
            Adapted.Proceed();
        }
    }
}
