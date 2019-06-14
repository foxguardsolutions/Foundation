using Autofac;

using FGS.Autofac.DynamicScoping.Abstractions;
using FGS.Pump.Extensions.DI;

namespace FGS.Pump.Eventing
{
    public static class ContainerBuilderExtensions
    {
        public static void RegisterEventUnhandled<TEvent>(this ContainerBuilder builder)
            where TEvent : Event
        {
            builder.RegisterType<LoggingNullEventHandler>().As<IEventHandler<TEvent>>().InstancePerDependency();
        }

        public static void RegisterEventHandler<TEvent, TEventHandler>(this ContainerBuilder builder, Scope scope)
            where TEventHandler : IEventHandler<TEvent>
            where TEvent : Event
        {
            builder.RegisterDecorator<PreEventHandlerTraceLoggingDecorator<TEvent, TEventHandler>, TEventHandler, IEventHandler<TEvent>>(scope);
        }
    }
}
