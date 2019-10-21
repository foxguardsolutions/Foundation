using System;
using System.Threading.Tasks;

using FGS.FaultHandling.Abstractions.Retry;

namespace FGS.FaultHandling.Polly.Retry
{
    public sealed class PollyRetryPolicyAdapter : IRetryPolicy
    {
        private readonly global::Polly.ISyncPolicy _syncPolicy;
        private readonly global::Polly.IAsyncPolicy _asyncPolicy;

        public PollyRetryPolicyAdapter(global::Polly.ISyncPolicy syncPolicy, global::Polly.IAsyncPolicy asyncPolicy)
        {
            _syncPolicy = syncPolicy;
            _asyncPolicy = asyncPolicy;
        }

        public void Execute(Action action)
        {
            _syncPolicy.Execute(action);
        }

        public TResult Execute<TResult>(Func<TResult> action) => _syncPolicy.Execute(action);

        public Task ExecuteAsync(Func<Task> action) => _asyncPolicy.ExecuteAsync(action);

        public Task<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> action) => _asyncPolicy.ExecuteAsync(action);
    }
}
