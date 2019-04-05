namespace FGS.Pump.Extensions
{
    public interface IFreezableClock
    {
        void FreezeTime();
        void UnfreezeTime();
    }
}