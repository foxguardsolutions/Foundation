using System;
using System.Threading.Tasks;

using FGS.FaultHandling.Abstractions.Retry;

namespace FGS.FaultHandling.Primitives.Retry
{
    /// <summary>
    /// An implementation of <see cref="IRetryPolicy"/> that doesn't do anything. It doesn't even retry anything.
    /// </summary>
    public sealed class NoOpRetryPolicy : IRetryPolicy
    {
        /// <inheritdoc />
        public void Execute(Action action)
        {
            action();
        }

        /// <inheritdoc />
        public TResult Execute<TResult>(Func<TResult> action) => action();

        /// <inheritdoc />
        public Task ExecuteAsync(Func<Task> action) => action();

        /// <inheritdoc />
        public Task<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> action) => action();
    }
}
