using System.Threading.Tasks;

using FGS.FaultHandling.Abstractions.Retry;
using FGS.Interception.Abstractions;

namespace FGS.Interceptors.FaultHandling.Retry
{
    /// <summary>
    /// An implementation of <see cref="IInterceptor"/> that uses an <see cref="IRetryPolicyCoordinator"/> to execute (and potentially
    /// retry) an invocation.
    /// </summary>
    public sealed class RetryInterceptor : IInterceptor
    {
        private readonly IRetryPolicyCoordinator _retryPolicyCoordinator;

        /// <summary>
        /// Initializes a new instance of the <see cref="RetryInterceptor"/> class.
        /// </summary>
        /// <param name="retryPolicyCoordinator">An instance of <see cref="IRetryPolicyCoordinator"/> that is used to invoke (and potentially retry) an invocation.</param>
        public RetryInterceptor(IRetryPolicyCoordinator retryPolicyCoordinator)
        {
            _retryPolicyCoordinator = retryPolicyCoordinator;
        }

        /// <inheritdoc />
        public void Intercept(IInvocation invocation)
        {
            var retryPolicy = _retryPolicyCoordinator.RequestPolicy();

            retryPolicy.Execute(invocation.Proceed);
        }

        /// <inheritdoc />
        public async Task InterceptAsync(IAsyncInvocation invocation)
        {
            var retryPolicy = _retryPolicyCoordinator.RequestPolicy();

            await retryPolicy.ExecuteAsync(async () => await invocation.ProceedAsync());
        }
    }
}
