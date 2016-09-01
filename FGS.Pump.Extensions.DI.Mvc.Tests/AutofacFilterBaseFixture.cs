using System;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Mvc.Async;

using Autofac;
using Autofac.Builder;

using FGS.Pump.Extensions.DI.Mvc.Tests.TestTypes;

using Moq;

using NUnit.Framework;

namespace FGS.Pump.Extensions.DI.Mvc.Tests
{
    /// <remarks>Taken and modified from: https://github.com/autofac/Autofac.Mvc/blob/e26ce3fe9ccc639f1349bcd8aee8e6e4ee066346/test/Autofac.Integration.Mvc.Test/AutofacFilterBaseFixture.cs </remarks>
    [TestFixture]
    public abstract class AutofacFilterBaseFixture<TFilter1, TFilter2, TFilterType>
        where TFilter1 : new()
        where TFilter2 : new()
    {
        private ControllerContext _baseControllerContext;
        private ControllerContext _derivedControllerContext;
        private ControllerContext _mostDerivedControllerContext;
        private ControllerDescriptor _controllerDescriptor;

        private MethodInfo _baseMethodInfo;
        private MethodInfo _derivedMethodInfo;
        private MethodInfo _mostDerivedMethodInfo;
        private string _actionName;

        private ReflectedActionDescriptor _reflectedActionDescriptor;
        private ReflectedAsyncActionDescriptor _reflectedAsyncActionDescriptor;
        private TaskAsyncActionDescriptor _taskAsyncActionDescriptor;
        private ReflectedActionDescriptor _derivedActionDescriptor;
        private ReflectedActionDescriptor _mostDerivedActionDescriptor;

        [OneTimeSetUp]
        public void FixtureSetUp()
        {
            _baseControllerContext = new ControllerContext { Controller = new TestController() };
            _derivedControllerContext = new ControllerContext { Controller = new TestControllerA() };
            _mostDerivedControllerContext = new ControllerContext { Controller = new TestControllerB() };

            _baseMethodInfo = TestController.GetAction1MethodInfo<TestController>();
            _derivedMethodInfo = TestController.GetAction1MethodInfo<TestControllerA>();
            _mostDerivedMethodInfo = TestController.GetAction1MethodInfo<TestControllerB>();
            _actionName = _baseMethodInfo.Name;

            _controllerDescriptor = new Mock<ControllerDescriptor>().Object;
            _reflectedActionDescriptor = new ReflectedActionDescriptor(_baseMethodInfo, _actionName, _controllerDescriptor);
            _reflectedAsyncActionDescriptor = new ReflectedAsyncActionDescriptor(_baseMethodInfo, _baseMethodInfo, _actionName, _controllerDescriptor);
            _taskAsyncActionDescriptor = new TaskAsyncActionDescriptor(_baseMethodInfo, _actionName, _controllerDescriptor);
            _derivedActionDescriptor = new ReflectedActionDescriptor(_derivedMethodInfo, _actionName, _controllerDescriptor);
            _mostDerivedActionDescriptor = new ReflectedActionDescriptor(_mostDerivedMethodInfo, _actionName, _controllerDescriptor);
        }

        [Test]
        public void ResolvesControllerScopedFilterForReflectedActionDescriptor()
        {
            AssertSingleFilter(
                FilterScope.Controller,
                _reflectedActionDescriptor,
                ConfigureFirstControllerRegistration());
        }

        [Test]
        public void ResolvesActionScopedFilterForReflectedActionDescriptor()
        {
            AssertSingleFilter(
                FilterScope.Action,
                _reflectedActionDescriptor,
                ConfigureFirstActionRegistration());
        }

        [Test]
        public void ResolvesActionScopedFilterForReflectedAsyncActionDescriptor()
        {
            AssertSingleFilter(
                FilterScope.Action,
                _reflectedAsyncActionDescriptor,
                ConfigureFirstActionRegistration());
        }

        [Test]
        public void ResolvesActionScopedFilterForTaskAsyncActionDescriptor()
        {
            AssertSingleFilter(
                FilterScope.Action,
                _taskAsyncActionDescriptor,
                ConfigureFirstActionRegistration());
        }

        [Test]
        public void ResolvesActionScopedFilterForImmediateBaseContoller()
        {
            AssertSingleFilter(
                FilterScope.Action,
                _derivedActionDescriptor,
                ConfigureFirstActionRegistration(),
                _derivedControllerContext);
        }

        [Test]
        public void ResolvesActionScopedFilterForMostBaseContoller()
        {
            AssertSingleFilter(
                FilterScope.Action,
                _mostDerivedActionDescriptor,
                ConfigureFirstActionRegistration(),
                _mostDerivedControllerContext);
        }

        [Test]
        public void ResolvesMultipleControllerScopedFilters()
        {
            AssertMultipleFilters(
                FilterScope.Controller,
                ConfigureFirstControllerRegistration(),
                ConfigureSecondControllerRegistration());
        }

        [Test]
        public void ResolvesMultipleActionScopedFilters()
        {
            AssertMultipleFilters(
                FilterScope.Action,
                ConfigureFirstActionRegistration(),
                ConfigureSecondActionRegistration());
        }

        [Test]
        public void ResolvesControllerScopedOverrideFilter()
        {
            AssertOverrideFilter(
                _reflectedActionDescriptor,
                ConfigureControllerFilterOverride());
        }

        [Test]
        public void ResolvesActionScopedOverrideFilterForReflectedActionDescriptor()
        {
            AssertOverrideFilter(
                _reflectedActionDescriptor,
                ConfigureActionFilterOverride());
        }

        [Test]
        public void ResolvesActionScopedOverrideFilterForReflectedAsyncActionDescriptor()
        {
            AssertOverrideFilter(
                _reflectedAsyncActionDescriptor,
                ConfigureActionFilterOverride());
        }

        [Test]
        public void ResolvesActionScopedOverrideFilterForTaskAsyncActionDescriptor()
        {
            AssertOverrideFilter(
                _taskAsyncActionDescriptor,
                ConfigureActionFilterOverride());
        }

        [Test]
        public void ResolvesActionScopedOverrideFilterForImmediateBaseContoller()
        {
            AssertOverrideFilter(
                _reflectedActionDescriptor,
                ConfigureActionFilterOverride(),
                _derivedControllerContext);
        }

        [Test]
        public void ResolvesActionScopedOverrideFilterForMostBaseContoller()
        {
            AssertOverrideFilter(
                _reflectedActionDescriptor,
                ConfigureActionFilterOverride(),
                _mostDerivedControllerContext);
        }

        [Test]
        public void ResolvesControllerScopedOverrideFilterForImmediateBaseContoller()
        {
            AssertOverrideFilter(
                _reflectedActionDescriptor,
                ConfigureControllerFilterOverride(),
                _derivedControllerContext);
        }

        [Test]
        public void ResolvesControllerScopedOverrideFilterForMostBaseContoller()
        {
            AssertOverrideFilter(
                _reflectedActionDescriptor,
                ConfigureControllerFilterOverride(),
                _mostDerivedControllerContext);
        }

        [Test]
        public void ResolvesRegisteredActionFilterOverrideForAction()
        {
            AssertFilterOverrideRegistration(
                FilterScope.Action,
                _reflectedActionDescriptor,
                ConfigureActionOverrideRegistration(),
                _baseControllerContext);
        }

        [Test]
        public void ResolvesRegisteredActionFilterOverrideForController()
        {
            AssertFilterOverrideRegistration(
                FilterScope.Controller,
                _reflectedActionDescriptor,
                ConfigureControllerOverrideRegistration(),
                _baseControllerContext);
        }

        protected abstract Action<IRegistrationBuilder<TFilter1, SimpleActivatorData, SingleRegistrationStyle>> ConfigureFirstControllerRegistration();

        protected abstract Action<IRegistrationBuilder<TFilter1, SimpleActivatorData, SingleRegistrationStyle>> ConfigureFirstActionRegistration();

        protected abstract Action<IRegistrationBuilder<TFilter2, SimpleActivatorData, SingleRegistrationStyle>> ConfigureSecondControllerRegistration();

        protected abstract Action<IRegistrationBuilder<TFilter2, SimpleActivatorData, SingleRegistrationStyle>> ConfigureSecondActionRegistration();

        protected abstract Action<ContainerBuilder> ConfigureControllerFilterOverride();

        protected abstract Action<ContainerBuilder> ConfigureActionFilterOverride();

        protected abstract Action<IRegistrationBuilder<TFilter1, SimpleActivatorData, SingleRegistrationStyle>> ConfigureActionOverrideRegistration();

        protected abstract Action<IRegistrationBuilder<TFilter1, SimpleActivatorData, SingleRegistrationStyle>> ConfigureControllerOverrideRegistration();

        protected abstract Type GetWrapperType();

        private static void SetupMockLifetimeScopeProvider(ILifetimeScope container)
        {
            var resolver = new Autofac.Integration.Mvc.AutofacDependencyResolver(container, new StubLifetimeScopeProvider(container));
            DependencyResolver.SetResolver(resolver);
        }

        private void AssertSingleFilter(
            FilterScope filterScope,
            ActionDescriptor actionDescriptor,
            Action<IRegistrationBuilder<TFilter1, SimpleActivatorData, SingleRegistrationStyle>> configure)
        {
            AssertSingleFilter(filterScope, actionDescriptor, configure, _baseControllerContext);
        }

        private static void AssertSingleFilter(
            FilterScope filterScope,
            ActionDescriptor actionDescriptor,
            Action<IRegistrationBuilder<TFilter1, SimpleActivatorData, SingleRegistrationStyle>> configure,
            ControllerContext controllerContext)
        {
            var builder = new ContainerBuilder();
            configure(builder.Register(c => new TFilter1()));
            var container = builder.Build();
            SetupMockLifetimeScopeProvider(container);
            var provider = new CustomAutofacFilterProvider();

            var filters = provider.GetFilters(controllerContext, actionDescriptor).ToList();

            Assert.That(filters, Has.Count.EqualTo(1));
            Assert.That(filters[0].Instance, Is.InstanceOf<TFilter1>());
            Assert.That(filters[0].Scope, Is.EqualTo(filterScope));
        }

        private void AssertMultipleFilters(
            FilterScope filterScope,
            Action<IRegistrationBuilder<TFilter1, SimpleActivatorData, SingleRegistrationStyle>> configure1,
            Action<IRegistrationBuilder<TFilter2, SimpleActivatorData, SingleRegistrationStyle>> configure2)
        {
            var builder = new ContainerBuilder();
            configure1(builder.Register(c => new TFilter1()));
            configure2(builder.Register(c => new TFilter2()));
            var container = builder.Build();
            SetupMockLifetimeScopeProvider(container);
            var actionDescriptor = new ReflectedActionDescriptor(_baseMethodInfo, _actionName, _controllerDescriptor);
            var provider = new CustomAutofacFilterProvider();

            var filters = provider.GetFilters(_baseControllerContext, actionDescriptor).ToList();

            Assert.That(filters, Has.Count.EqualTo(2));

            var filter = filters.Single(f => f.Instance is TFilter1);
            Assert.That(filter.Scope, Is.EqualTo(filterScope));
            Assert.That(filter.Order, Is.EqualTo(Filter.DefaultOrder));

            filter = filters.Single(f => f.Instance is TFilter2);
            Assert.That(filter.Scope, Is.EqualTo(filterScope));
            Assert.That(filter.Order, Is.EqualTo(20));
        }

        private void AssertOverrideFilter(ActionDescriptor actionDescriptor, Action<ContainerBuilder> registration)
        {
            AssertOverrideFilter(actionDescriptor, registration, _baseControllerContext);
        }

        private static void AssertOverrideFilter(ActionDescriptor actionDescriptor, Action<ContainerBuilder> registration, ControllerContext controllerContext)
        {
            var builder = new ContainerBuilder();
            registration(builder);
            var container = builder.Build();
            SetupMockLifetimeScopeProvider(container);
            var provider = new CustomAutofacFilterProvider();

            var filters = provider.GetFilters(controllerContext, actionDescriptor).ToList();

            var filter = filters.Select(info => info.Instance).OfType<CustomAutofacOverrideFilter>().Single();
            Assert.That(filter, Is.InstanceOf<CustomAutofacOverrideFilter>());
            Assert.That(filter.FiltersToOverride, Is.EqualTo(typeof(TFilterType)));
        }

        private void AssertFilterOverrideRegistration(
            FilterScope filterScope,
            ActionDescriptor actionDescriptor,
            Action<IRegistrationBuilder<TFilter1, SimpleActivatorData, SingleRegistrationStyle>> configure,
            ControllerContext controllerContext)
        {
            var builder = new ContainerBuilder();
            configure(builder.Register(c => new TFilter1()));
            var container = builder.Build();
            SetupMockLifetimeScopeProvider(container);
            var provider = new CustomAutofacFilterProvider();

            var filters = provider.GetFilters(controllerContext, actionDescriptor).ToList();

            Assert.That(filters, Has.Count.EqualTo(1));
            Assert.That(filters[0].Instance, Is.InstanceOf(GetWrapperType()));
            Assert.That(filters[0].Scope, Is.EqualTo(filterScope));
        }
    }
}
