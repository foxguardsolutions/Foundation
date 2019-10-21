namespace FGS.FaultHandling.Abstractions.Retry
{
    public interface IRetryPolicyCoordinator
    {
        IRetryPolicy RequestPolicy();
    }
}