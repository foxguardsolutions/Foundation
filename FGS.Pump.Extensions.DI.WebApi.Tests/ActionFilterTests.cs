using System;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

using Autofac;
using Autofac.Builder;

namespace FGS.Pump.Extensions.DI.WebApi.Tests
{
    /// <remarks>Taken and modified from: https://github.com/autofac/Autofac.WebApi/blob/f764f7e10694a57cf19c968c1ca5b6b998ba82c2/test/Autofac.Integration.WebApi.Test/ActionFilterFixture.cs </remarks>
    public class ActionFilterTests : AutofacFilterBaseFixture<TestActionFilter, TestActionFilter2, IActionFilter>
    {
        private readonly Func<HttpControllerDescriptor, HttpActionDescriptor, bool> _controllerPredicate = (hcd, had) => typeof(TestController).IsAssignableFrom(hcd.ControllerType);
        private readonly Func<HttpControllerDescriptor, HttpActionDescriptor, bool> _actionPredicate = (hcd, had) => typeof(TestController).IsAssignableFrom(hcd.ControllerType) && had.ActionName == nameof(TestController.Get);

        protected override Func<IComponentContext, TestActionFilter> GetFirstRegistration()
        {
            return c => new TestActionFilter(c.Resolve<ILogger>());
        }

        protected override Func<IComponentContext, TestActionFilter2> GetSecondRegistration()
        {
            return c => new TestActionFilter2(c.Resolve<ILogger>());
        }

        protected override Action<IRegistrationBuilder<TestActionFilter, SimpleActivatorData, SingleRegistrationStyle>> ConfigureFirstControllerRegistration()
        {
            return r => r.AsWebApiActionFilterWhen((hcd, had) => typeof(TestController).IsAssignableFrom(hcd.ControllerType), FilterScope.Controller);
        }

        protected override Action<IRegistrationBuilder<TestActionFilter, SimpleActivatorData, SingleRegistrationStyle>> ConfigureFirstActionRegistration()
        {
            return r => r.AsWebApiActionFilterWhen(_actionPredicate, FilterScope.Action);
        }

        protected override Action<IRegistrationBuilder<TestActionFilter2, SimpleActivatorData, SingleRegistrationStyle>> ConfigureSecondControllerRegistration()
        {
            return r => r.AsWebApiActionFilterWhen((hcd, had) => typeof(TestController).IsAssignableFrom(hcd.ControllerType), FilterScope.Controller);
        }

        protected override Action<IRegistrationBuilder<TestActionFilter2, SimpleActivatorData, SingleRegistrationStyle>> ConfigureSecondActionRegistration()
        {
            return r => r.AsWebApiActionFilterWhen(_actionPredicate, FilterScope.Action);
        }

        protected override Type GetWrapperType()
        {
            return typeof(ActionFilterWrapper);
        }

        protected override Type GetOverrideWrapperType()
        {
            return typeof(ActionFilterOverrideWrapper);
        }

        protected override Action<ContainerBuilder> ConfigureControllerFilterOverride()
        {
            return builder => builder.RegisterWebApiActionFilterOverrideForWhen(_controllerPredicate, FilterScope.Controller);
        }

        protected override Action<ContainerBuilder> ConfigureActionFilterOverride()
        {
            return builder => builder.RegisterWebApiActionFilterOverrideForWhen(_actionPredicate, FilterScope.Action);
        }

        protected override Action<IRegistrationBuilder<TestActionFilter, SimpleActivatorData, SingleRegistrationStyle>> ConfigureActionOverrideRegistration()
        {
            return r => r.AsWebApiActionFilterOverrideWhen(_actionPredicate, FilterScope.Action);
        }

        protected override Action<IRegistrationBuilder<TestActionFilter, SimpleActivatorData, SingleRegistrationStyle>> ConfigureControllerOverrideRegistration()
        {
            return r => r.AsWebApiActionFilterOverrideWhen(_controllerPredicate, FilterScope.Controller);
        }
    }
}
