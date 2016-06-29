using System.Reflection;
using System.Threading.Tasks;

using Ninject.Extensions.Interception;

namespace FGS.Pump.Extensions.DI
{
    /// <summary>
    /// A simple definition of an interceptor, which can take action both before and after
    /// the invocation proceeds and supports async methods.
    /// </summary>
    /// <remarks>Taken and modified slightly from: https://github.com/ninject/Ninject.Extensions.Interception/blob/d84db87b15ff675d27b7b8d493e8148d45801910/src/Ninject.Extensions.Interception/AsyncInterceptor.cs </remarks>
    public abstract class NonRacingAsyncInterceptor : IInterceptor
    {
        private static readonly MethodInfo StartTaskMethodInfo = typeof(NonRacingAsyncInterceptor).GetMethod(nameof(InterceptTaskWithResult), BindingFlags.Instance | BindingFlags.NonPublic);

        /// <summary>
        /// Intercepts the specified invocation.
        /// </summary>
        /// <param name="invocation">The invocation to intercept.</param>
        public void Intercept(IInvocation invocation)
        {
            var returnType = invocation.Request.Method.ReturnType;
            if (returnType == typeof(Task))
            {
                InterceptTask(invocation);
                return;
            }

            if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                var resultType = returnType.GetGenericArguments()[0];
                var mi = StartTaskMethodInfo.MakeGenericMethod(resultType);
                mi.Invoke(this, new object[] { invocation });
                return;
            }

            BeforeInvoke(invocation);
            invocation.Proceed();
            AfterInvoke(invocation);
        }

        private void InterceptTask(IInvocation invocation)
        {
            var invocationClone = invocation.Clone();
            var resultTask =
                Task.Run(() => BeforeInvoke(invocation))
                .ContinueWith(t =>
                {
                    invocationClone.Proceed();
                    return invocationClone.ReturnValue as Task;
                }).Unwrap()
                .ContinueWith(t =>
                {
                    AfterInvoke(invocation);
                    AfterInvoke(invocation, t);
                });
            invocation.ReturnValue = resultTask;
            resultTask.Wait();
        }

        private void InterceptTaskWithResult<TResult>(IInvocation invocation)
        {
            var invocationClone = invocation.Clone();
            var resultTask =
                Task.Run(() => BeforeInvoke(invocation))
                .ContinueWith(t =>
                {
                    invocationClone.Proceed();
                    return invocationClone.ReturnValue as Task<TResult>;
                }).Unwrap()
                .ContinueWith(t =>
                {
                    invocationClone.ReturnValue = t.Result;
                    AfterInvoke(invocationClone);
                    AfterInvoke(invocationClone, t);
                    return (TResult)invocationClone.ReturnValue;
                });
            invocation.ReturnValue = resultTask;
            resultTask.Wait();
        }

        /// <summary>
        /// Takes some action before the invocation proceeds.
        /// </summary>
        /// <param name="invocation">The invocation that is being intercepted.</param>
        protected virtual void BeforeInvoke(IInvocation invocation)
        {
        }

        /// <summary>
        /// Takes some action after the invocation proceeds.
        /// </summary>
        /// <remarks>Use one AfterInvoke method overload.</remarks>
        /// <param name="invocation">The invocation that is being intercepted.</param>
        protected virtual void AfterInvoke(IInvocation invocation)
        {
        }

        /// <summary>
        /// Takes some action after the invocation proceeds.
        /// </summary>
        /// <remarks>Use one AfterInvoke method overload.</remarks>
        /// <param name="invocation">The invocation that is being intercepted.</param>
        /// <param name="task">The task that was executed.</param>
        protected virtual void AfterInvoke(IInvocation invocation, Task task)
        {
        }
    }
}
