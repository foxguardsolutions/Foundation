namespace FGS.Pump.Eventing
{
    public class NullEventHandler<TEvent> : IEventHandler<TEvent>
        where TEvent : Event
    {
        #region Implementation of IEventHandler<in TEvent>

        public void Handle(TEvent eventPayload)
        {
        }

        #endregion
    }
}