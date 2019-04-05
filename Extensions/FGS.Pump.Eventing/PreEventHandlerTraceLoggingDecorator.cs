using FGS.Pump.Logging;
using FGS.Pump.Logging.Decorators;

namespace FGS.Pump.Eventing
{
    public class PreEventHandlerTraceLoggingDecorator<TEvent, TDecorated> : LoggingDecorator<TDecorated>, IEventHandler<TEvent>
        where TEvent : Event
        where TDecorated : IEventHandler<TEvent>
    {
        public PreEventHandlerTraceLoggingDecorator(TDecorated decorated, IStructuralLoggerBuilder structuralLoggerBuilder)
            : base(decorated, structuralLoggerBuilder)
        {
        }

        #region Implementation of IEventHandler<in TEvent>

        public void Handle(TEvent eventPayload)
        {
            Logger.Debug("Handling {event}", eventPayload);
            Decorated.Handle(eventPayload);
        }

        #endregion
    }
}