using System;
using System.Web.Mvc;

namespace FGS.Pump.Extensions.DI.Mvc
{
    internal class CustomFilterMetadata
    {
        private CustomFilterMetadata(Func<ControllerContext, bool> controllerPredicate, Func<ControllerContext, ActionDescriptor, bool> actionPredicate, FilterScope filterScope, int order)
        {
            ControllerPredicate = controllerPredicate;
            ActionPredicate = actionPredicate;
            FilterScope = filterScope;
            Order = order;
        }

        public CustomFilterMetadata(Func<ControllerContext, bool> controllerPredicate, FilterScope filterScope, int order)
            : this(controllerPredicate, (cc, ad) => true, filterScope, order)
        {
        }

        public CustomFilterMetadata(Func<ControllerContext, ActionDescriptor, bool> actionPredicate, FilterScope filterScope, int order)
            : this(cc => true, actionPredicate, filterScope, order)
        {
        }

        public Func<ControllerContext, bool> ControllerPredicate { get; }

        public Func<ControllerContext, ActionDescriptor, bool> ActionPredicate { get; }

        public FilterScope FilterScope { get; }

        public int Order { get; }
    }
}