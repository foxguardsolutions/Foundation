using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Threading.Tasks;

namespace FGS.Pump.Extensions
{
    public interface IHistoricalReactiveProcessor
    {
        Task<IEnumerable<Timestamped<TResult>>> MapAsync<TInput, TResult>(IEnumerable<Timestamped<TInput>> inputs, Func<IObservable<TInput>, IScheduler, IObservable<TResult>> mapper);
    }
}