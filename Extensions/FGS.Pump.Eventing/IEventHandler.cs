namespace FGS.Pump.Eventing
{
    public interface IEventHandler<in TEvent>
        where TEvent : Event
    {
        void Handle(TEvent eventPayload);
    }
}