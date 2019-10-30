using System;
using System.Threading.Tasks;

using FGS.FaultHandling.Abstractions.Retry;

namespace FGS.FaultHandling.Primitives.Retry
{
    public sealed class ObservableRetryPolicyDecorator : IRetryPolicy
    {
        private readonly IRetryPolicy _decorated;
        private readonly Action _afterExecute;

        public ObservableRetryPolicyDecorator(IRetryPolicy decorated, Action afterExecute)
        {
            _decorated = decorated;
            _afterExecute = afterExecute;
        }

        public void Execute(Action action)
        {
            _decorated.Execute(action);
            _afterExecute();
        }

        public TResult Execute<TResult>(Func<TResult> action)
        {
            var result = _decorated.Execute(action);
            _afterExecute();
            return result;
        }

        public async Task ExecuteAsync(Func<Task> action)
        {
            await _decorated.ExecuteAsync(action);
            _afterExecute();
        }

        public async Task<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> action)
        {
            var result = await _decorated.ExecuteAsync(action);
            _afterExecute();
            return result;
        }
    }
}
