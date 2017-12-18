using Castle.DynamicProxy;

namespace FGS.Pump.FaultHandling.Interception
{
    public class RetryInterceptor : IInterceptor
    {
        private readonly IRetryPolicyCoordinator _retryPolicyCoordinator;

        public RetryInterceptor(IRetryPolicyCoordinator retryPolicyCoordinator, RetryOnFaultAttribute attribute)
        {
            _retryPolicyCoordinator = retryPolicyCoordinator;
        }

        public void Intercept(IInvocation invocation)
        {
            var retryPolicy = _retryPolicyCoordinator.RequestPolicy();

            retryPolicy.Execute(invocation.Proceed);
        }
    }
}