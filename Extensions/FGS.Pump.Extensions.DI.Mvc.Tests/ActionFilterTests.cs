using System;
using System.Web.Mvc;

using Autofac;
using Autofac.Builder;

using FGS.Pump.Extensions.DI.Mvc.Tests.TestTypes;

namespace FGS.Pump.Extensions.DI.Mvc.Tests
{
    /// <remarks>Taken and modified from: https://github.com/autofac/Autofac.Mvc/blob/e26ce3fe9ccc639f1349bcd8aee8e6e4ee066346/test/Autofac.Integration.Mvc.Test/ActionFilterFixture.cs s</remarks>
    public class ActionFilterTests : AutofacFilterBaseFixture<TestActionFilter, TestActionFilter2, IActionFilter>
    {
        protected override Action<IRegistrationBuilder<TestActionFilter, SimpleActivatorData, SingleRegistrationStyle>> ConfigureFirstControllerRegistration()
        {
            return r => r.AsActionFilterWhen((cc, ad) => cc.Controller is TestController, FilterScope.Controller, order: -1);
        }

        protected override Action<IRegistrationBuilder<TestActionFilter, SimpleActivatorData, SingleRegistrationStyle>> ConfigureFirstActionRegistration()
        {
            return r => r.AsActionFilterWhen((cc, ad) => cc.Controller is TestController && ad.ActionName == nameof(TestController.Action1), FilterScope.Action, order: -1);
        }

        protected override Action<IRegistrationBuilder<TestActionFilter2, SimpleActivatorData, SingleRegistrationStyle>> ConfigureSecondControllerRegistration()
        {
            return r => r.AsActionFilterWhen((cc, ad) => cc.Controller is TestController, FilterScope.Controller, order: 20);
        }

        protected override Action<IRegistrationBuilder<TestActionFilter2, SimpleActivatorData, SingleRegistrationStyle>> ConfigureSecondActionRegistration()
        {
            return r => r.AsActionFilterWhen((cc, ad) => cc.Controller is TestController && ad.ActionName == nameof(TestController.Action1), FilterScope.Action, order: 20);
        }

        protected override Action<ContainerBuilder> ConfigureControllerFilterOverride()
        {
            return builder => builder.RegisterActionFilterOverrideForWhen((cc, ad) => cc.Controller is TestController, FilterScope.Action);
        }

        protected override Action<ContainerBuilder> ConfigureActionFilterOverride()
        {
            return builder => builder.RegisterActionFilterOverrideForWhen((cc, ad) => cc.Controller is TestController && ad.ActionName == nameof(TestController.Action1), FilterScope.Action);
        }

        protected override Action<IRegistrationBuilder<TestActionFilter, SimpleActivatorData, SingleRegistrationStyle>> ConfigureActionOverrideRegistration()
        {
            return r => r.AsActionFilterOverrideWhen((cc, ad) => cc.Controller is TestController && ad.ActionName == nameof(TestController.Action1), FilterScope.Action, order: 0);
        }

        protected override Action<IRegistrationBuilder<TestActionFilter, SimpleActivatorData, SingleRegistrationStyle>> ConfigureControllerOverrideRegistration()
        {
            return r => r.AsActionFilterOverrideWhen((cc, ad) => cc.Controller is TestController, FilterScope.Controller, order: 0);
        }

        protected override Type GetWrapperType()
        {
            return typeof(ActionFilterOverride);
        }
    }
}
