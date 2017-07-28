using Castle.DynamicProxy;

namespace FGS.Pump.FaultHandling.Interception
{
    public class RetryInterceptor : IInterceptor
    {
        private readonly IRetryPolicyCoordinator _retryPolicyCoordinator;
        private readonly RetryOnFaultAttribute _attribute;

        public RetryInterceptor(IRetryPolicyCoordinator retryPolicyCoordinator, RetryOnFaultAttribute attribute)
        {
            _retryPolicyCoordinator = retryPolicyCoordinator;
            _attribute = attribute;
        }

        public void Intercept(IInvocation invocation)
        {
            var retryPolicy = _retryPolicyCoordinator.RequestPolicy();

            retryPolicy.Execute(invocation.Proceed);
        }
    }
}