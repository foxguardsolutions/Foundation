using System;
using System.Collections.Generic;

namespace FGS.FaultHandling.Abstractions.Retry
{
    /// <summary>
    /// Represents a factory of <see cref="IRetryPolicy"/> instances that takes exception predicates as a parameter.
    /// </summary>
    public interface IRetryPolicyFactory
    {
        /// <summary>
        /// Creates an instance of <see cref="IRetryPolicy"/>.
        /// </summary>
        /// <param name="exceptionPredicates">Predicates of which one of which must evaluate to <see langword="true" /> for the returned <see cref="IRetryPolicy"/> to execute a retry.</param>
        /// <returns>Returns an <see cref="IRetryPolicy"/>.</returns>
        IRetryPolicy Create(IEnumerable<Func<Exception, bool>> exceptionPredicates);
    }
}
