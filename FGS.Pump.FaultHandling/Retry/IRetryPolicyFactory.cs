using System;
using System.Collections.Generic;

namespace FGS.Pump.FaultHandling.Retry
{
    public interface IRetryPolicyFactory
    {
        IRetryPolicy Create(IEnumerable<Func<Exception, bool>> exceptionPredicates);
    }
}