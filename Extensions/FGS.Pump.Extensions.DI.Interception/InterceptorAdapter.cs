using System;
using System.Threading.Tasks;

using ICastleInterceptor = Castle.DynamicProxy.IInterceptor;
using ICastleInterceptorInvocation = Castle.DynamicProxy.IInvocation;

namespace FGS.Pump.Extensions.DI.Interception
{
    internal class InterceptorAdapter<TInterceptor, TInterceptorInstantiationData> : ICastleInterceptor
        where TInterceptor : IInterceptor
    {
        private readonly Func<ICastleInterceptorInvocation, TInterceptorInstantiationData> _interceptorInstantiationDataFactory;
        private readonly Func<TInterceptorInstantiationData, TInterceptor> _adaptedFactory;
        private readonly Func<ICastleInterceptorInvocation, IInvocation> _invocationAdapterFactory;
        private readonly Func<ICastleInterceptorInvocation, IAsyncInvocation> _asyncInvocationAdapterFactory;

        public InterceptorAdapter(
            Func<ICastleInterceptorInvocation, TInterceptorInstantiationData> interceptorInstantiationDataFactory,
            Func<TInterceptorInstantiationData, TInterceptor> adaptedFactory,
            Func<ICastleInterceptorInvocation, IInvocation> invocationAdapterFactory,
            Func<ICastleInterceptorInvocation, IAsyncInvocation> asyncInvocationAdapterFactory)
        {
            _interceptorInstantiationDataFactory = interceptorInstantiationDataFactory;
            _adaptedFactory = adaptedFactory;
            _invocationAdapterFactory = invocationAdapterFactory;
            _asyncInvocationAdapterFactory = asyncInvocationAdapterFactory;
        }

        void ICastleInterceptor.Intercept(ICastleInterceptorInvocation invocation)
        {
            var instantiationData = _interceptorInstantiationDataFactory(invocation);
            var adapted = _adaptedFactory(instantiationData);

            if (!typeof(Task).IsAssignableFrom(invocation.Method.ReturnType))
            {
                var adaptedInvocation = _invocationAdapterFactory(invocation);

                adapted.Intercept(adaptedInvocation);
            }
            else
            {
                Task adaptedContinuation = null;
                var adaptedInvocation = _asyncInvocationAdapterFactory(invocation);

                try
                {
                    adaptedContinuation = adapted.InterceptAsync(adaptedInvocation);
                }
                catch (Exception e)
                {
                    // NOTE: We can get here if the intercepted code throws an exception _before_ beginning an `await`
                    invocation.ReturnValue = Task.FromException(e);
                }

                if (adaptedContinuation != null)
                {
                    InterceptorTaskConnector.Connect(invocation, adaptedInvocation.ReturnValue, adaptedContinuation);
                }
            }
        }
    }
}
