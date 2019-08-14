using System.Reflection;
using System.Threading.Tasks;

using ICastleInterceptorInvocation = Castle.DynamicProxy.IInvocation;

namespace FGS.Pump.Extensions.DI.Interception
{
    internal static class InterceptorTaskConnector
    {
        private static readonly MethodInfo ConnectMethodInfo = typeof(InterceptorTaskConnector).GetMethod(nameof(InnerConnect), BindingFlags.Static | BindingFlags.NonPublic);

        public static void Connect(ICastleInterceptorInvocation invocation, object returnValue, Task task)
        {
            var returnType = invocation.Method.ReturnType;

            if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                var resultType = returnType.GetGenericArguments()[0];
                var methodInfo = ConnectMethodInfo.MakeGenericMethod(resultType);
                methodInfo.Invoke(null, new object[] { invocation, returnValue, task });
            }
            else
            {
#pragma warning disable CA2008 // Do not create tasks without passing a TaskScheduler
                invocation.ReturnValue = task.ContinueWith(x => returnValue);
#pragma warning restore CA2008 // Do not create tasks without passing a TaskScheduler
            }
        }

        private static void InnerConnect<T>(ICastleInterceptorInvocation invocation, object returnValue, Task adaptedContinuation)
        {
#pragma warning disable CA2008 // Do not create tasks without passing a TaskScheduler
            invocation.ReturnValue = adaptedContinuation.ContinueWith(async x => await (Task<T>)returnValue).Unwrap();
#pragma warning restore CA2008 // Do not create tasks without passing a TaskScheduler
        }
    }
}
