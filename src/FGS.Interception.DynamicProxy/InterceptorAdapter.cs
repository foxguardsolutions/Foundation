using System;
using System.Threading.Tasks;

using FGS.Interception.Abstractions;

using ICastleInterceptor = Castle.DynamicProxy.IInterceptor;
using ICastleInterceptorInvocation = Castle.DynamicProxy.IInvocation;

namespace FGS.Interception.DynamicProxy
{
    /// <summary>
    /// Adapts our <typeparamref name="TInterceptor"/> interceptor as an implementation of <see cref="ICastleInterceptor"/>.
    /// </summary>
    /// <typeparam name="TInterceptor">The implementation type of <see cref="IInterceptor"/> being adapted.</typeparam>
    /// <typeparam name="TInterceptorInstantiationData">The type of data that should be provided to the underlying <typeparamref name="TInterceptor"/> when it is being instantiated.</typeparam>
    public sealed class InterceptorAdapter<TInterceptor, TInterceptorInstantiationData> : ICastleInterceptor
        where TInterceptor : IInterceptor
    {
        private readonly Func<ICastleInterceptorInvocation, TInterceptorInstantiationData> _interceptorInstantiationDataFactory;
        private readonly Func<TInterceptorInstantiationData, TInterceptor> _adaptedFactory;
        private readonly Func<ICastleInterceptorInvocation, IInvocation> _invocationAdapterFactory;
        private readonly Func<ICastleInterceptorInvocation, IAsyncInvocation> _asyncInvocationAdapterFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="InterceptorAdapter{TInterceptor, TInterceptorInstantiationData}"/> class.
        /// </summary>
        /// <param name="interceptorInstantiationDataFactory">A factory that can retrieve or create an instance of <typeparamref name="TInterceptorInstantiationData"/> from an inbound <see cref="ICastleInterceptorInvocation"/>.</param>
        /// <param name="adaptedFactory">A factory that can retrieve or create an instance of <typeparamref name="TInterceptor"/> based on a given <typeparamref name="TInterceptorInstantiationData"/>.</param>
        /// <param name="invocationAdapterFactory">A factory that can retrieve or create an instance of <see cref="IInvocation"/> based on an inbound <see cref="ICastleInterceptorInvocation"/>.</param>
        /// <param name="asyncInvocationAdapterFactory">A factory that can retrieve or create an instance of <see cref="IAsyncInvocation"/> based on an inbound <see cref="ICastleInterceptorInvocation"/>.</param>
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

        /// <inheritdoc />
        /// <remarks>
        /// <para>
        ///   ⚠🐛 If <value>_interceptorInstantiationDataFactory</value> or <value>_adaptedFactory</value> throw exceptions, (which
        ///   can result from misconfigured dependency injection), it can be difficult to detect based on how this class and its
        ///   collaborators interact. They have been known to manifest as difficult-to-explain <see cref="NullReferenceException"/>s thrown
        ///   during sociable testing or production runtime conditions.
        /// </para>
        /// <para>
        ///   🏗ℹ Ideally, this implementation would catch possible exceptions from the factories that are called during setup - and then
        ///   chain them into the returned continuation - but such would increase the complexity of this type beyond what has been time-budgeted
        ///   each time we've happened to be refactoring near it.
        /// </para>
        /// </remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "For interceptors, upstream exception types are unknowable by definition")]
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
