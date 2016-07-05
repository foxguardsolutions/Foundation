using System;
using System.Threading.Tasks;

using Castle.DynamicProxy;

namespace FGS.Pump.Extensions.DI.Interception
{
    internal class AsyncAwareSwitchingInterceptor<TSyncInterceptor, TAsyncInterceptor> : IInterceptor
        where TSyncInterceptor : IInterceptor
        where TAsyncInterceptor : NonRacingAsyncInterceptor
    {
        private Lazy<TSyncInterceptor> _lazySyncInterceptor;
        private Lazy<TAsyncInterceptor> _lazyAsyncInterceptor;

        public AsyncAwareSwitchingInterceptor(Lazy<TSyncInterceptor> lazySyncInterceptor, Lazy<TAsyncInterceptor> lazyAsyncInterceptor)
        {
            _lazySyncInterceptor = lazySyncInterceptor;
            _lazyAsyncInterceptor = lazyAsyncInterceptor;
        }

        public void Intercept(IInvocation invocation)
        {
            if (typeof(Task).IsAssignableFrom(invocation.GetConcreteMethodInvocationTarget().ReturnType))
                _lazyAsyncInterceptor.Value.Intercept(invocation);
            else
                _lazySyncInterceptor.Value.Intercept(invocation);
        }
    }
}