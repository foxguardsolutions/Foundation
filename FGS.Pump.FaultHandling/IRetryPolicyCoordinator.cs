using FGS.Pump.FaultHandling.Retry;

namespace FGS.Pump.FaultHandling
{
    public interface IRetryPolicyCoordinator
    {
        IRetryPolicy RequestPolicy();
    }
}