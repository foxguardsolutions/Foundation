using System;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

using Autofac;
using Autofac.Builder;

namespace FGS.Pump.Extensions.DI.WebApi.Tests
{
    /// <remarks>Taken and modified from: https://github.com/autofac/Autofac.WebApi/blob/f764f7e10694a57cf19c968c1ca5b6b998ba82c2/test/Autofac.Integration.WebApi.Test/AuthorizationFilterFixture.cs </remarks>
    public class AuthorizationFilterTests : AutofacFilterBaseFixture<TestAuthorizationFilter, TestAuthorizationFilter2, IAuthorizationFilter>
    {
        private readonly Func<HttpControllerDescriptor, HttpActionDescriptor, bool> _controllerPredicate = (hcd, had) => typeof(TestController).IsAssignableFrom(hcd.ControllerType);
        private readonly Func<HttpControllerDescriptor, HttpActionDescriptor, bool> _actionPredicate = (hcd, had) => typeof(TestController).IsAssignableFrom(hcd.ControllerType) && had.ActionName == nameof(TestController.Get);

        protected override Func<IComponentContext, TestAuthorizationFilter> GetFirstRegistration()
        {
            return c => new TestAuthorizationFilter(c.Resolve<ILogger>());
        }

        protected override Func<IComponentContext, TestAuthorizationFilter2> GetSecondRegistration()
        {
            return c => new TestAuthorizationFilter2(c.Resolve<ILogger>());
        }

        protected override Action<IRegistrationBuilder<TestAuthorizationFilter, SimpleActivatorData, SingleRegistrationStyle>> ConfigureFirstControllerRegistration()
        {
            return r => r.AsWebApiAuthorizationFilterWhen((hcd, had) => typeof(TestController).IsAssignableFrom(hcd.ControllerType), FilterScope.Controller, order: 0);
        }

        protected override Action<IRegistrationBuilder<TestAuthorizationFilter, SimpleActivatorData, SingleRegistrationStyle>> ConfigureFirstActionRegistration()
        {
            return r => r.AsWebApiAuthorizationFilterWhen(_actionPredicate, FilterScope.Action, order: 0);
        }

        protected override Action<IRegistrationBuilder<TestAuthorizationFilter2, SimpleActivatorData, SingleRegistrationStyle>> ConfigureSecondControllerRegistration()
        {
            return r => r.AsWebApiAuthorizationFilterWhen((hcd, had) => typeof(TestController).IsAssignableFrom(hcd.ControllerType), FilterScope.Controller, order: 0);
        }

        protected override Action<IRegistrationBuilder<TestAuthorizationFilter2, SimpleActivatorData, SingleRegistrationStyle>> ConfigureSecondActionRegistration()
        {
            return r => r.AsWebApiAuthorizationFilterWhen(_actionPredicate, FilterScope.Action, order: 0);
        }

        protected override Type GetWrapperType()
        {
            return typeof(AuthorizationFilterWrapper);
        }

        protected override Type GetOverrideWrapperType()
        {
            return typeof(AuthorizationFilterOverrideWrapper);
        }

        protected override Action<ContainerBuilder> ConfigureControllerFilterOverride()
        {
            return builder => builder.RegisterWebApiAuthorizationFilterOverrideForWhen(_controllerPredicate, FilterScope.Controller, order: 0);
        }

        protected override Action<ContainerBuilder> ConfigureActionFilterOverride()
        {
            return builder => builder.RegisterWebApiAuthorizationFilterOverrideForWhen(_actionPredicate, FilterScope.Action, order: 0);
        }

        protected override Action<IRegistrationBuilder<TestAuthorizationFilter, SimpleActivatorData, SingleRegistrationStyle>> ConfigureActionOverrideRegistration()
        {
            return r => r.AsWebApiAuthorizationFilterOverrideWhen(_actionPredicate, FilterScope.Action, order: 0);
        }

        protected override Action<IRegistrationBuilder<TestAuthorizationFilter, SimpleActivatorData, SingleRegistrationStyle>> ConfigureControllerOverrideRegistration()
        {
            return r => r.AsWebApiAuthorizationFilterOverrideWhen(_controllerPredicate, FilterScope.Controller, order: 0);
        }
    }
}
