using System.Threading.Tasks;

using FGS.FaultHandling.Abstractions.Retry;
using FGS.Interception.Abstractions;

namespace FGS.Interception.FaultHandling.Retry
{
    public class RetryInterceptor : IInterceptor
    {
        private readonly IRetryPolicyCoordinator _retryPolicyCoordinator;

        public RetryInterceptor(IRetryPolicyCoordinator retryPolicyCoordinator)
        {
            _retryPolicyCoordinator = retryPolicyCoordinator;
        }

        public void Intercept(IInvocation invocation)
        {
            var retryPolicy = _retryPolicyCoordinator.RequestPolicy();

            retryPolicy.Execute(invocation.Proceed);
        }

        public async Task InterceptAsync(IAsyncInvocation invocation)
        {
            var retryPolicy = _retryPolicyCoordinator.RequestPolicy();

            await retryPolicy.ExecuteAsync(async () => await invocation.ProceedAsync());
        }
    }
}
