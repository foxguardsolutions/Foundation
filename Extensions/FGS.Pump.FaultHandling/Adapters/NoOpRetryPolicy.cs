using System;
using System.Threading.Tasks;

using FGS.Pump.FaultHandling.Retry;

namespace FGS.Pump.FaultHandling.Adapters
{
    internal sealed class NoOpRetryPolicy : IRetryPolicy
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