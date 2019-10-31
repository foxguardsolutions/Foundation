using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Threading.Tasks;

namespace FGS.Rx.Extensions.Abstractions
{
    /// <summary>
    /// Represents the capability to take a collection of timestamped events, process it with Rx operators, and await the materialization of the result back into a collection.
    /// </summary>
    public interface IHistoricalReactiveProcessor
    {
        /// <summary>
        /// Takes a collection of timestamped events, processes it with Rx operators, and then allows the caller to await the materialization of the result back into a collection.
        /// </summary>
        /// <typeparam name="TInput">The type of events that are the input.</typeparam>
        /// <typeparam name="TResult">The type of events that are the output.</typeparam>
        /// <param name="inputs">The input collection of timestamped events.</param>
        /// <param name="mapper">A function representing an Rx-based transformation of inputs into outputs.</param>
        /// <returns>An awaitable materialization of timestamped events.</returns>
        Task<IEnumerable<Timestamped<TResult>>> MapAsync<TInput, TResult>(IEnumerable<Timestamped<TInput>> inputs, Func<IObservable<TInput>, IScheduler, IObservable<TResult>> mapper);
    }
}
