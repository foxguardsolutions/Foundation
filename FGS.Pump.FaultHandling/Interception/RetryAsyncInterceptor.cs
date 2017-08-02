using System.Reflection;
using System.Threading.Tasks;

using Castle.DynamicProxy;

using FGS.Pump.Extensions.DI.Interception;
using FGS.Pump.FaultHandling.Retry;

namespace FGS.Pump.FaultHandling.Interception
{
    public class RetryAsyncInterceptor : NonRacingAsyncInterceptor
    {
        private readonly MethodInfo _invokeAsyncFuncMethodInfo = typeof(RetryAsyncInterceptor).GetMethod(nameof(InvokeAsyncFunc), BindingFlags.Instance | BindingFlags.NonPublic);
        private readonly MethodInfo _invokeFuncMethodInfo = typeof(RetryAsyncInterceptor).GetMethod(nameof(InvokeFunc), BindingFlags.Instance | BindingFlags.NonPublic);

        private readonly IRetryPolicyCoordinator _retryPolicyCoordinator;
        private readonly RetryOnFaultAttribute _attribute;

        public RetryAsyncInterceptor(IRetryPolicyCoordinator retryPolicyCoordinator, RetryOnFaultAttribute attribute)
        {
            _retryPolicyCoordinator = retryPolicyCoordinator;
            _attribute = attribute;
        }

        protected override void Invoke(IInvocation invocation)
        {
            var retryPolicy = _retryPolicyCoordinator.RequestPolicy();

            var returnType = invocation.MethodInvocationTarget.ReturnType;
            if (returnType == typeof(Task))
            {
               var resultTask = retryPolicy.ExecuteAsync(async () => await Task.Run(() => invocation.Proceed()));
                invocation.ReturnValue = resultTask;
                resultTask.Wait();
                return;
            }

            if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                var resultType = returnType.GetGenericArguments()[0];
                var methodInfo = _invokeAsyncFuncMethodInfo.MakeGenericMethod(resultType);
                methodInfo.Invoke(this, parameters: new object[] { invocation, retryPolicy });
                return;
            }

            if (returnType.IsGenericType)
            {
                var resultType = returnType.GetGenericArguments()[0];
                var methodInfo = _invokeFuncMethodInfo.MakeGenericMethod(resultType);
                methodInfo.Invoke(this, parameters: new object[] { invocation, retryPolicy });
                return;
            }

            retryPolicy.Execute(invocation.Proceed);
        }

        private void InvokeAsyncFunc<TResult>(IInvocation invocation, IRetryPolicy retryPolicy)
        {
            var resultTask = retryPolicy.ExecuteAsync(() =>
            {
                invocation.Proceed();
                return (Task<TResult>)invocation.ReturnValue;
            });

            invocation.ReturnValue = resultTask;
            resultTask.Wait();
        }

        private void InvokeFunc<TResult>(IInvocation invocation, IRetryPolicy retryPolicy)
        {
            retryPolicy.Execute<TResult>(() =>
            {
                invocation.Proceed();
                return (TResult)invocation.ReturnValue;
            });
        }
    }
}