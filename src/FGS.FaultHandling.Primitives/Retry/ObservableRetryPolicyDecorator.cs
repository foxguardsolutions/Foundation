using System;
using System.Threading.Tasks;

using FGS.FaultHandling.Abstractions.Retry;

namespace FGS.FaultHandling.Primitives.Retry
{
    /// <summary>
    /// An implementation of <see cref="IRetryPolicy"/> that decorates an inner implementation and allows an observer
    /// to be aware of when an execution attempt (including retries) is performed.
    /// </summary>
    public sealed class ObservableRetryPolicyDecorator : IRetryPolicy
    {
        private readonly IRetryPolicy _decorated;
        private readonly Action _afterExecute;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableRetryPolicyDecorator"/> class.
        /// </summary>
        /// <param name="decorated">The decorated implementation of <see cref="IRetryPolicy"/> that is to be observed.</param>
        /// <param name="afterExecute">The <see cref="Action"/> that will be called in order to signal to the observer than an execution attempt has been performed.</param>
        public ObservableRetryPolicyDecorator(IRetryPolicy decorated, Action afterExecute)
        {
            _decorated = decorated;
            _afterExecute = afterExecute;
        }

        /// <inheritdoc />
        public void Execute(Action action)
        {
            _decorated.Execute(action);
            _afterExecute();
        }

        /// <inheritdoc />
        public TResult Execute<TResult>(Func<TResult> action)
        {
            var result = _decorated.Execute(action);
            _afterExecute();
            return result;
        }

        /// <inheritdoc />
        public async Task ExecuteAsync(Func<Task> action)
        {
            await _decorated.ExecuteAsync(action);
            _afterExecute();
        }

        /// <inheritdoc />
        public async Task<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> action)
        {
            var result = await _decorated.ExecuteAsync(action);
            _afterExecute();
            return result;
        }
    }
}
