using System;
using System.Collections.Generic;
using System.Linq;

using FGS.Pump.FaultHandling.Adapters;

namespace FGS.Pump.FaultHandling.Retry
{
    internal sealed class RetryPolicyCoordinator : IRetryPolicyCoordinator
    {
        private readonly object _trackingSyncLock = new object();
        private readonly IRetryPolicyFactory _retryPolicyFactory;
        private readonly Func<NoOpRetryPolicy> _noOpFactory;
        private readonly IEnumerable<IExceptionRetryPredicate> _exceptionPredicates;

        private int _callStackDepth = 0;

        public RetryPolicyCoordinator(IRetryPolicyFactory retryPolicyFactory, Func<NoOpRetryPolicy> noOpFactory, IEnumerable<IExceptionRetryPredicate> exceptionPredicates)
        {
            _retryPolicyFactory = retryPolicyFactory;
            _noOpFactory = noOpFactory;
            _exceptionPredicates = exceptionPredicates;
        }

        public IRetryPolicy RequestPolicy()
        {
            return UseTrackingSyncLock(
                () =>
                {
                    _callStackDepth++;

                    IRetryPolicy retryPolicy;
                    if (_callStackDepth == 1)
                    {
                        var exceptionPredicates = GetExceptionPredicates();
                        retryPolicy = _retryPolicyFactory.Create(exceptionPredicates);
                    }
                    else
                    {
                        retryPolicy = _noOpFactory();
                    }

                    return new ObservableRetryPolicyDecorator(retryPolicy, afterExecute: DecrementCallStackDepth);
                });
        }

        private IEnumerable<Func<Exception, bool>> GetExceptionPredicates()
        {
            return UseTrackingSyncLock(
                () => _exceptionPredicates.Select<IExceptionRetryPredicate, Func<Exception, bool>>(
                    predicate => (Exception ex) => predicate.ShouldRetry(ex)));
        }

        private void DecrementCallStackDepth()
        {
            UseTrackingSyncLock(() => _callStackDepth--);
        }

        private TResult UseTrackingSyncLock<TResult>(Func<TResult> action)
        {
            lock (_trackingSyncLock)
            {
                return action();
            }
        }
    }
}