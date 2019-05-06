using System.Threading.Tasks;

using FGS.Pump.Extensions.DI.Interception;

namespace FGS.Pump.FaultHandling.Interception
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
