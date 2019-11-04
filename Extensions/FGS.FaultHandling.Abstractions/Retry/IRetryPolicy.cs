using System;
using System.Threading.Tasks;

namespace FGS.FaultHandling.Abstractions.Retry
{
    /// <summary>
    /// Represents a retry policy.
    /// </summary>
    public interface IRetryPolicy
    {
        /// <summary>
        /// Execute the given action within the retry policy.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        void Execute(Action action);

        /// <summary>
        /// Execute the given func within the retry policy.
        /// </summary>
        /// <typeparam name="TResult">The return type of the <paramref name="action"/>.</typeparam>
        /// <param name="action">The action to execute.</param>
        /// <returns>The return value of <paramref name="action"/>.</returns>
        TResult Execute<TResult>(Func<TResult> action);

        /// <summary>
        /// Execute the given async action within the retry policy.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <returns>A <see cref="Task"/> pointing to the <paramref name="action"/>.</returns>
        Task ExecuteAsync(Func<Task> action);

        /// <summary>
        /// Execute the given async func within the retry policy.
        /// </summary>
        /// <typeparam name="TResult">The return type of the <paramref name="action"/>.</typeparam>
        /// <param name="action">The action to execute.</param>
        /// <returns>A <see cref="Task{TResult}"/> pointing to the return value of <paramref name="action"/>.</returns>
        Task<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> action);
    }
}
