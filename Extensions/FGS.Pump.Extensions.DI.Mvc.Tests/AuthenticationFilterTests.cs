using System;
using System.Web.Mvc;
using System.Web.Mvc.Filters;

using Autofac;
using Autofac.Builder;

using FGS.Pump.Extensions.DI.Mvc.Tests.TestTypes;

namespace FGS.Pump.Extensions.DI.Mvc.Tests
{
    /// <remarks>Taken and modified from: https://github.com/autofac/Autofac.Mvc/blob/e26ce3fe9ccc639f1349bcd8aee8e6e4ee066346/test/Autofac.Integration.Mvc.Test/AuthenticationFilterFixture.cs </remarks>>
    public class AuthenticationFilterTests : AutofacFilterBaseFixture<TestAuthenticationFilter, TestAuthenticationFilter2, IAuthenticationFilter>
    {
        protected override Action<IRegistrationBuilder<TestAuthenticationFilter, SimpleActivatorData, SingleRegistrationStyle>> ConfigureFirstControllerRegistration()
        {
            return r => r.AsAuthenticationFilterWhen((cc, ad) => cc.Controller is TestController, FilterScope.Controller, order: -1);
        }

        protected override Action<IRegistrationBuilder<TestAuthenticationFilter, SimpleActivatorData, SingleRegistrationStyle>> ConfigureFirstActionRegistration()
        {
            return r => r.AsAuthenticationFilterWhen((cc, ad) => cc.Controller is TestController && ad.ActionName == nameof(TestController.Action1), FilterScope.Action, order: -1);
        }

        protected override Action<IRegistrationBuilder<TestAuthenticationFilter2, SimpleActivatorData, SingleRegistrationStyle>> ConfigureSecondControllerRegistration()
        {
            return r => r.AsAuthenticationFilterWhen((cc, ad) => cc.Controller is TestController, FilterScope.Controller, order: 20);
        }

        protected override Action<IRegistrationBuilder<TestAuthenticationFilter2, SimpleActivatorData, SingleRegistrationStyle>> ConfigureSecondActionRegistration()
        {
            return r => r.AsAuthenticationFilterWhen((cc, ad) => cc.Controller is TestController && ad.ActionName == nameof(TestController.Action1), FilterScope.Action, order: 20);
        }

        protected override Action<ContainerBuilder> ConfigureControllerFilterOverride()
        {
            return builder => builder.RegisterAuthenticationFilterOverrideForWhen((cc, ad) => cc.Controller is TestController, FilterScope.Action);
        }

        protected override Action<ContainerBuilder> ConfigureActionFilterOverride()
        {
            return builder => builder.RegisterAuthenticationFilterOverrideForWhen((cc, ad) => cc.Controller is TestController && ad.ActionName == nameof(TestController.Action1), FilterScope.Action);
        }

        protected override Action<IRegistrationBuilder<TestAuthenticationFilter, SimpleActivatorData, SingleRegistrationStyle>> ConfigureActionOverrideRegistration()
        {
            return r => r.AsAuthenticationFilterOverrideWhen((cc, ad) => cc.Controller is TestController && ad.ActionName == nameof(TestController.Action1), FilterScope.Action, order: 0);
        }

        protected override Action<IRegistrationBuilder<TestAuthenticationFilter, SimpleActivatorData, SingleRegistrationStyle>> ConfigureControllerOverrideRegistration()
        {
            return r => r.AsAuthenticationFilterOverrideWhen((cc, ad) => cc.Controller is TestController, FilterScope.Controller, order: 0);
        }

        protected override Type GetWrapperType()
        {
            return typeof(AuthenticationFilterOverride);
        }
    }
}
