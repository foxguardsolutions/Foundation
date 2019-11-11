using System;
using System.Collections.Generic;
using System.Linq;

using FGS.FaultHandling.Abstractions.Retry;

namespace FGS.FaultHandling.Primitives.Retry
{
    /// <summary>
    /// An implementation of <see cref="IRetryPolicyCoordinator"/> that satisfies requests for <see cref="IRetryPolicy"/> by
    /// checking first-time exceptions against <see cref="IExceptionRetryPredicate"/> instances, in order to determine whether
    /// or not a _retrying_ retry policy is returned to the caller.
    /// </summary>
    public sealed class RetryPolicyCoordinator : IRetryPolicyCoordinator
    {
        private readonly object _trackingSyncLock = new object();
        private readonly IRetryPolicyFactory _retryPolicyFactory;
        private readonly Func<NoOpRetryPolicy> _noOpFactory;
        private readonly IEnumerable<IExceptionRetryPredicate> _exceptionPredicates;

        private int _callStackDepth;

        /// <summary>
        /// Initializes a new instance of the <see cref="RetryPolicyCoordinator"/> class.
        /// </summary>
        /// <param name="retryPolicyFactory">A factory that is used to retrieve instances of <see cref="IRetryPolicy"/> that can be used to retry operations.</param>
        /// <param name="noOpFactory">A factory that is used to retrieve instances of <see cref="NoOpRetryPolicy"/> which can be used to prevent operations from being retried.</param>
        /// <param name="exceptionPredicates">The conditions indicating whether or not an operation that failed from a given exception should be retried.</param>
        public RetryPolicyCoordinator(IRetryPolicyFactory retryPolicyFactory, Func<NoOpRetryPolicy> noOpFactory, IEnumerable<IExceptionRetryPredicate> exceptionPredicates)
        {
            _retryPolicyFactory = retryPolicyFactory;
            _noOpFactory = noOpFactory;
            _exceptionPredicates = exceptionPredicates;
        }

        /// <inheritdoc />
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
                    predicate => predicate.ShouldRetry));
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
