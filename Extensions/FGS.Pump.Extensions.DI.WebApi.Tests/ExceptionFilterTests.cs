using System;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

using Autofac;
using Autofac.Builder;

using FGS.Pump.Extensions.DI.WebApi.Tests.TestTypes;

namespace FGS.Pump.Extensions.DI.WebApi.Tests
{
    /// <remarks>Taken and modified from: https://github.com/autofac/Autofac.WebApi/blob/f764f7e10694a57cf19c968c1ca5b6b998ba82c2/test/Autofac.Integration.WebApi.Test/ExceptionFilterFixture.cs </remarks>
    public class ExceptionFilterTests : AutofacFilterBaseFixture<TestExceptionFilter, TestExceptionFilter2, IExceptionFilter>
    {
        private readonly Func<HttpControllerDescriptor, HttpActionDescriptor, bool> _controllerPredicate = (hcd, had) => typeof(TestController).IsAssignableFrom(hcd.ControllerType);
        private readonly Func<HttpControllerDescriptor, HttpActionDescriptor, bool> _actionPredicate = (hcd, had) => typeof(TestController).IsAssignableFrom(hcd.ControllerType) && had.ActionName == nameof(TestController.Get);

        protected override Func<IComponentContext, TestExceptionFilter> GetFirstRegistration()
        {
            return c => new TestExceptionFilter(c.Resolve<ILogger>());
        }

        protected override Func<IComponentContext, TestExceptionFilter2> GetSecondRegistration()
        {
            return c => new TestExceptionFilter2(c.Resolve<ILogger>());
        }

        protected override Action<IRegistrationBuilder<TestExceptionFilter, SimpleActivatorData, SingleRegistrationStyle>> ConfigureFirstControllerRegistration()
        {
            return r => r.AsWebApiExceptionFilterWhen((hcd, had) => typeof(TestController).IsAssignableFrom(hcd.ControllerType), FilterScope.Controller, order: 0);
        }

        protected override Action<IRegistrationBuilder<TestExceptionFilter, SimpleActivatorData, SingleRegistrationStyle>> ConfigureFirstActionRegistration()
        {
            return r => r.AsWebApiExceptionFilterWhen(_actionPredicate, FilterScope.Action, order: 0);
        }

        protected override Action<IRegistrationBuilder<TestExceptionFilter2, SimpleActivatorData, SingleRegistrationStyle>> ConfigureSecondControllerRegistration()
        {
            return r => r.AsWebApiExceptionFilterWhen((hcd, had) => typeof(TestController).IsAssignableFrom(hcd.ControllerType), FilterScope.Controller, order: 0);
        }

        protected override Action<IRegistrationBuilder<TestExceptionFilter2, SimpleActivatorData, SingleRegistrationStyle>> ConfigureSecondActionRegistration()
        {
            return r => r.AsWebApiExceptionFilterWhen(_actionPredicate, FilterScope.Action, order: 0);
        }

        protected override Type GetWrapperType()
        {
            return typeof(ExceptionFilterWrapper);
        }

        protected override Type GetOverrideWrapperType()
        {
            return typeof(ExceptionFilterOverrideWrapper);
        }

        protected override Action<ContainerBuilder> ConfigureControllerFilterOverride()
        {
            return builder => builder.RegisterWebApiExceptionFilterOverrideForWhen(_controllerPredicate, FilterScope.Controller, order: 0);
        }

        protected override Action<ContainerBuilder> ConfigureActionFilterOverride()
        {
            return builder => builder.RegisterWebApiExceptionFilterOverrideForWhen(_actionPredicate, FilterScope.Action, order: 0);
        }

        protected override Action<IRegistrationBuilder<TestExceptionFilter, SimpleActivatorData, SingleRegistrationStyle>> ConfigureActionOverrideRegistration()
        {
            return r => r.AsWebApiExceptionFilterOverrideWhen(_actionPredicate, FilterScope.Action, order: 0);
        }

        protected override Action<IRegistrationBuilder<TestExceptionFilter, SimpleActivatorData, SingleRegistrationStyle>> ConfigureControllerOverrideRegistration()
        {
            return r => r.AsWebApiExceptionFilterOverrideWhen(_controllerPredicate, FilterScope.Controller, order: 0);
        }
    }
}
