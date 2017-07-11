using System;
using System.Threading.Tasks;

using FGS.Pump.FaultHandling.Retry;

namespace FGS.Pump.FaultHandling.Adapters
{
    internal sealed class PollyRetryPolicyAdapter : IRetryPolicy
    {
        private readonly Polly.ISyncPolicy _syncPolicy;
        private readonly Polly.IAsyncPolicy _asyncPolicy;

        public PollyRetryPolicyAdapter(Polly.ISyncPolicy syncPolicy, Polly.IAsyncPolicy asyncPolicy)
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