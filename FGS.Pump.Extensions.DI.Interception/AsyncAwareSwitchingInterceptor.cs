using System;
using System.Threading.Tasks;

using Castle.DynamicProxy;

namespace FGS.Pump.Extensions.DI.Interception
{
    internal class AsyncAwareSwitchingInterceptor<TSyncInterceptor, TAsyncInterceptor, TInterceptorInstanciationData> : IInterceptor
        where TSyncInterceptor : IInterceptor
        where TAsyncInterceptor : NonRacingAsyncInterceptor
    {
        private readonly Func<TInterceptorInstanciationData, TSyncInterceptor> _syncInterceptorFactory;
        private readonly Func<TInterceptorInstanciationData, TAsyncInterceptor> _asyncInterceptorFactory;
        private readonly Func<IInvocation, TInterceptorInstanciationData> _interceptorInstanciationDataFactory;

        public AsyncAwareSwitchingInterceptor(Func<TInterceptorInstanciationData, TSyncInterceptor> syncInterceptorFactory, Func<TInterceptorInstanciationData, TAsyncInterceptor> asyncInterceptorFactory, Func<IInvocation, TInterceptorInstanciationData> interceptorInstanciationDataFactory)
        {
            _syncInterceptorFactory = syncInterceptorFactory;
            _asyncInterceptorFactory = asyncInterceptorFactory;
            _interceptorInstanciationDataFactory = interceptorInstanciationDataFactory;
        }

        public void Intercept(IInvocation invocation)
        {
            var interceptorInstanciationData = _interceptorInstanciationDataFactory(invocation);

            if (typeof(Task).IsAssignableFrom(invocation.GetConcreteMethodInvocationTarget().ReturnType))
                _asyncInterceptorFactory(interceptorInstanciationData).Intercept(invocation);
            else
                _syncInterceptorFactory(interceptorInstanciationData).Intercept(invocation);
        }
    }
}