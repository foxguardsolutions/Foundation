using System;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

using Autofac;
using Autofac.Builder;

namespace FGS.Pump.Extensions.DI.WebApi.Tests
{
    /// <remarks>Taken and modified from: https://github.com/autofac/Autofac.WebApi/blob/f764f7e10694a57cf19c968c1ca5b6b998ba82c2/test/Autofac.Integration.WebApi.Test/AuthenticationFilterFixture.cs </remarks>
    public class AuthenticationFilterTests : AutofacFilterBaseFixture<TestAuthenticationFilter, TestAuthenticationFilter2, IAuthenticationFilter>
    {
        private readonly Func<HttpControllerDescriptor, HttpActionDescriptor, bool> _controllerPredicate = (hcd, had) => typeof(TestController).IsAssignableFrom(hcd.ControllerType);
        private readonly Func<HttpControllerDescriptor, HttpActionDescriptor, bool> _actionPredicate = (hcd, had) => typeof(TestController).IsAssignableFrom(hcd.ControllerType) && had.ActionName == nameof(TestController.Get);

        protected override Func<IComponentContext, TestAuthenticationFilter> GetFirstRegistration()
        {
            return c => new TestAuthenticationFilter(c.Resolve<ILogger>());
        }

        protected override Func<IComponentContext, TestAuthenticationFilter2> GetSecondRegistration()
        {
            return c => new TestAuthenticationFilter2(c.Resolve<ILogger>());
        }

        protected override Action<IRegistrationBuilder<TestAuthenticationFilter, SimpleActivatorData, SingleRegistrationStyle>> ConfigureFirstControllerRegistration()
        {
            return r => r.AsWebApiAuthenticationFilterWhen((hcd, had) => typeof(TestController).IsAssignableFrom(hcd.ControllerType), FilterScope.Controller, order: 0);
        }

        protected override Action<IRegistrationBuilder<TestAuthenticationFilter, SimpleActivatorData, SingleRegistrationStyle>> ConfigureFirstActionRegistration()
        {
            return r => r.AsWebApiAuthenticationFilterWhen(_actionPredicate, FilterScope.Action, order: 0);
        }

        protected override Action<IRegistrationBuilder<TestAuthenticationFilter2, SimpleActivatorData, SingleRegistrationStyle>> ConfigureSecondControllerRegistration()
        {
            return r => r.AsWebApiAuthenticationFilterWhen((hcd, had) => typeof(TestController).IsAssignableFrom(hcd.ControllerType), FilterScope.Controller, order: 0);
        }

        protected override Action<IRegistrationBuilder<TestAuthenticationFilter2, SimpleActivatorData, SingleRegistrationStyle>> ConfigureSecondActionRegistration()
        {
            return r => r.AsWebApiAuthenticationFilterWhen(_actionPredicate, FilterScope.Action, order: 0);
        }

        protected override Type GetWrapperType()
        {
            return typeof(AuthenticationFilterWrapper);
        }

        protected override Type GetOverrideWrapperType()
        {
            return typeof(AuthenticationFilterOverrideWrapper);
        }

        protected override Action<ContainerBuilder> ConfigureControllerFilterOverride()
        {
            return builder => builder.RegisterWebApiAuthenticationFilterOverrideForWhen(_controllerPredicate, FilterScope.Controller, order: 0);
        }

        protected override Action<ContainerBuilder> ConfigureActionFilterOverride()
        {
            return builder => builder.RegisterWebApiAuthenticationFilterOverrideForWhen(_actionPredicate, FilterScope.Action, order: 0);
        }

        protected override Action<IRegistrationBuilder<TestAuthenticationFilter, SimpleActivatorData, SingleRegistrationStyle>> ConfigureActionOverrideRegistration()
        {
            return r => r.AsWebApiAuthenticationFilterOverrideWhen(_actionPredicate, FilterScope.Action, order: 0);
        }

        protected override Action<IRegistrationBuilder<TestAuthenticationFilter, SimpleActivatorData, SingleRegistrationStyle>> ConfigureControllerOverrideRegistration()
        {
            return r => r.AsWebApiAuthenticationFilterOverrideWhen(_controllerPredicate, FilterScope.Controller, order: 0);
        }
    }
}
