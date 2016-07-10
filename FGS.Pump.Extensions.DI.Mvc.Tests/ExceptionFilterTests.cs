using System;
using System.Web.Mvc;

using Autofac;
using Autofac.Builder;

using FGS.Pump.Extensions.DI.Mvc.Tests.TestTypes;

namespace FGS.Pump.Extensions.DI.Mvc.Tests
{
    public class ExceptionFilterTests : AutofacFilterBaseFixture<TestExceptionFilter, TestExceptionFilter2, IExceptionFilter>
    {
        protected override Action<IRegistrationBuilder<TestExceptionFilter, SimpleActivatorData, SingleRegistrationStyle>> ConfigureFirstControllerRegistration()
        {
            return r => r.AsExceptionFilterWhen((cc, ad) => cc.Controller is TestController, FilterScope.Controller, order: -1);
        }

        protected override Action<IRegistrationBuilder<TestExceptionFilter, SimpleActivatorData, SingleRegistrationStyle>> ConfigureFirstActionRegistration()
        {
            return r => r.AsExceptionFilterWhen((cc, ad) => cc.Controller is TestController && ad.ActionName == nameof(TestController.Action1), FilterScope.Action, order: -1);
        }

        protected override Action<IRegistrationBuilder<TestExceptionFilter2, SimpleActivatorData, SingleRegistrationStyle>> ConfigureSecondControllerRegistration()
        {
            return r => r.AsExceptionFilterWhen((cc, ad) => cc.Controller is TestController, FilterScope.Controller, order: 20);
        }

        protected override Action<IRegistrationBuilder<TestExceptionFilter2, SimpleActivatorData, SingleRegistrationStyle>> ConfigureSecondActionRegistration()
        {
            return r => r.AsExceptionFilterWhen((cc, ad) => cc.Controller is TestController && ad.ActionName == nameof(TestController.Action1), FilterScope.Action, order: 20);
        }

        protected override Action<ContainerBuilder> ConfigureControllerFilterOverride()
        {
            return builder => builder.RegisterExceptionFilterOverrideForWhen((cc, ad) => cc.Controller is TestController, FilterScope.Action);
        }

        protected override Action<ContainerBuilder> ConfigureActionFilterOverride()
        {
            return builder => builder.RegisterExceptionFilterOverrideForWhen((cc, ad) => cc.Controller is TestController && ad.ActionName == nameof(TestController.Action1), FilterScope.Action);
        }

        protected override Action<IRegistrationBuilder<TestExceptionFilter, SimpleActivatorData, SingleRegistrationStyle>> ConfigureActionOverrideRegistration()
        {
            return r => r.AsExceptionFilterOverrideWhen((cc, ad) => cc.Controller is TestController && ad.ActionName == nameof(TestController.Action1), FilterScope.Action, order: 0);
        }

        protected override Action<IRegistrationBuilder<TestExceptionFilter, SimpleActivatorData, SingleRegistrationStyle>> ConfigureControllerOverrideRegistration()
        {
            return r => r.AsExceptionFilterOverrideWhen((cc, ad) => cc.Controller is TestController, FilterScope.Controller, order: 0);
        }

        protected override Type GetWrapperType()
        {
            return typeof(ExceptionFilterOverride);
        }
    }
}
