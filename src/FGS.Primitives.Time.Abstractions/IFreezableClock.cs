namespace FGS.Primitives.Time.Abstractions
{
    public interface IFreezableClock
    {
        void FreezeTime();
        void UnfreezeTime();
    }
}
