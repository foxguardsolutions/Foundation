using System;
using System.Threading.Tasks;

using FGS.FaultHandling.Abstractions.Retry;

namespace FGS.FaultHandling.Polly.Retry
{
    /// <summary>
    /// Implements an <see cref="IRetryPolicy"/> using Polly.
    /// </summary>
    public sealed class PollyRetryPolicyAdapter : IRetryPolicy
    {
        private readonly global::Polly.ISyncPolicy _syncPolicy;
        private readonly global::Polly.IAsyncPolicy _asyncPolicy;

        /// <summary>
        /// Initializes a new instance of the <see cref="PollyRetryPolicyAdapter"/> class.
        /// </summary>
        /// <param name="syncPolicy">The synchronous half of the policy's implementation.</param>
        /// <param name="asyncPolicy">The asynchronous half of the policy's implementation.</param>
        public PollyRetryPolicyAdapter(global::Polly.ISyncPolicy syncPolicy, global::Polly.IAsyncPolicy asyncPolicy)
        {
            _syncPolicy = syncPolicy;
            _asyncPolicy = asyncPolicy;
        }

        /// <inheritdoc/>
        public void Execute(Action action)
        {
            _syncPolicy.Execute(action);
        }

        /// <inheritdoc/>
        public TResult Execute<TResult>(Func<TResult> action) => _syncPolicy.Execute(action);

        /// <inheritdoc/>
        public Task ExecuteAsync(Func<Task> action) => _asyncPolicy.ExecuteAsync(action);

        /// <inheritdoc/>
        public Task<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> action) => _asyncPolicy.ExecuteAsync(action);
    }
}
