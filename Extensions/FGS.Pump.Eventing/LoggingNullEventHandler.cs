using FGS.Pump.Logging;

namespace FGS.Pump.Eventing
{
    /// <remarks>
    /// Tossed this together because the logging decorators that use the decorated type's name for the logger name results
    /// in nearly unreadable logger names when the decorated type is itself generic.
    /// </remarks>
    public class LoggingNullEventHandler : IEventHandler<Event>
    {
        private readonly IStructuralLogger _logger;

        public LoggingNullEventHandler(IStructuralLoggerBuilder structuralLoggerBuilder)
        {
            _logger = structuralLoggerBuilder.ForContext(GetType()).Create();
        }

        #region Implementation of IEventHandler<in Event>

        public void Handle(Event eventPayload)
        {
            _logger.Debug("Handle {eventPayload}", eventPayload);
        }

        #endregion
    }
}
