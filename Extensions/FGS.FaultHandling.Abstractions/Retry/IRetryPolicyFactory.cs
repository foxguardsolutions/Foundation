using System;
using System.Collections.Generic;

namespace FGS.FaultHandling.Abstractions.Retry
{
    public interface IRetryPolicyFactory
    {
        IRetryPolicy Create(IEnumerable<Func<Exception, bool>> exceptionPredicates);
    }
}