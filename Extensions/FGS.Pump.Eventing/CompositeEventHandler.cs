using System.Collections.Generic;
using System.Linq;

namespace FGS.Pump.Eventing
{
    public class CompositeEventHandler<TEvent> : IEventHandler<TEvent>
        where TEvent : Event
    {
        private readonly IEnumerable<IEventHandler<TEvent>> _innerHandlers;

        public CompositeEventHandler(IEnumerable<IEventHandler<TEvent>> innerHandlers)
        {
            _innerHandlers = innerHandlers;
        }

        public void Handle(TEvent eventPayload)
        {
            _innerHandlers.ToList().ForEach(h => h.Handle(eventPayload));
        }
    }
}
