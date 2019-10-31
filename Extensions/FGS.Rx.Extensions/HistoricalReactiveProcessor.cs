using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;

using FGS.Rx.Extensions.Abstractions;

namespace FGS.Rx.Extensions
{
    /// <inheritdoc/>
    public class HistoricalReactiveProcessor : IHistoricalReactiveProcessor
    {
        private readonly Func<HistoricalScheduler> _historicalSchedulerFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="HistoricalReactiveProcessor"/> class.
        /// </summary>
        /// <param name="historicalSchedulerFactory">A factory that creates or retrieves an instance of <see cref="HistoricalScheduler"/> that is used to process the stream of events by ignoring the normal flow of time.</param>
        public HistoricalReactiveProcessor(Func<HistoricalScheduler> historicalSchedulerFactory)
        {
            _historicalSchedulerFactory = historicalSchedulerFactory;
        }

        /// <inheritdoc/>
        public Task<IEnumerable<Timestamped<TResult>>> MapAsync<TInput, TResult>(IEnumerable<Timestamped<TInput>> inputs, Func<IObservable<TInput>, IScheduler, IObservable<TResult>> mapper)
        {
            Task<IEnumerable<Timestamped<TResult>>> result;

            var scheduler = _historicalSchedulerFactory();

            var orderedInputs = inputs.OrderBy(x => x.Timestamp).ToArray();
            using (var subject = new ReplaySubject<TInput>(scheduler))
            {
                foreach (var input in orderedInputs)
                {
                    scheduler.ScheduleAbsolute(input.Timestamp, () => subject.OnNext(input.Value));
                }

                scheduler.ScheduleAbsolute(DateTimeOffset.MaxValue, () => subject.OnCompleted());

                result = mapper(subject, scheduler).Timestamp(scheduler).ToArray().Cast<IEnumerable<Timestamped<TResult>>>().ToTask();

                scheduler.Start();
            }

            return result;
        }
    }
}
