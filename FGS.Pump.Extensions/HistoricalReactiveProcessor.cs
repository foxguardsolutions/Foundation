using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;

namespace FGS.Pump.Extensions
{
    public class HistoricalReactiveProcessor : IHistoricalReactiveProcessor
    {
        private readonly Func<HistoricalScheduler> _historicalSchedulerFactory;

        public HistoricalReactiveProcessor(Func<HistoricalScheduler> historicalSchedulerFactory)
        {
            _historicalSchedulerFactory = historicalSchedulerFactory;
        }

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
