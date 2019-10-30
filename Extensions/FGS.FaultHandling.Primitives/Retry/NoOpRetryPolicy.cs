using System;
using System.Threading.Tasks;

using FGS.FaultHandling.Abstractions.Retry;

namespace FGS.FaultHandling.Primitives.Retry
{
    public sealed class NoOpRetryPolicy : IRetryPolicy
    {
        public void Execute(Action action)
        {
            action();
        }

        public TResult Execute<TResult>(Func<TResult> action) => action();

        public Task ExecuteAsync(Func<Task> action) => action();

        public Task<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> action) => action();
    }
}
